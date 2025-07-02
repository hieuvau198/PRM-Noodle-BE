using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Models;
using Services.DTOs.AIChat;
using Services.DTOs.Product;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;
        private readonly string _geminiEndpoint;

        public ChatService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<ChatService> logger,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _geminiApiKey = _configuration["GeminiAPI:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured");
            _geminiEndpoint = _configuration["GeminiAPI:Endpoint"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";
        }

        public async Task<ChatResponseDto> ProcessChatMessageAsync(ChatRequestDto request)
        {
            try
            {
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

                // Generate product context
                var productContext = await GenerateProductContextAsync();

                // Create system prompt
                var systemPrompt = CreateSystemPrompt(productContext);

                // Build the complete prompt
                var completePrompt = $"{systemPrompt}\n\nUser Question: {request.Message}";

                // Call Gemini API
                var geminiResponse = await CallGeminiApiAsync(completePrompt);

                // Parse response and extract suggested products if any
                var (response, suggestedProducts, isProductQuery) = await ParseGeminiResponseAsync(geminiResponse, request.Message);

                return new ChatResponseDto
                {
                    Response = response,
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow,
                    SuggestedProducts = suggestedProducts,
                    IsProductQuery = isProductQuery
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message: {Message}", request.Message);
                return new ChatResponseDto
                {
                    Response = "I apologize, but I'm having trouble processing your request right now. Please try again later.",
                    SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    IsProductQuery = false
                };
            }
        }

        public async Task<string> GenerateProductContextAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.FindAsync(p => p.IsAvailable == true);
                var availableProducts = products.ToList();

                if (!availableProducts.Any())
                {
                    return "Currently, there are no available products in our inventory.";
                }

                var contextBuilder = new StringBuilder();
                contextBuilder.AppendLine("Available Products in Database:");
                contextBuilder.AppendLine("===============================");

                foreach (var product in availableProducts.OrderBy(p => p.BasePrice))
                {
                    contextBuilder.AppendLine($"- Product ID: {product.ProductId}");
                    contextBuilder.AppendLine($"  Name: {product.ProductName}");
                    contextBuilder.AppendLine($"  Price: ${product.BasePrice:F2}");
                    contextBuilder.AppendLine($"  Description: {product.Description ?? "No description"}");
                    contextBuilder.AppendLine($"  Spice Level: {product.SpiceLevel ?? "Not specified"}");
                    contextBuilder.AppendLine($"  Available: {(product.IsAvailable == true ? "Yes" : "No")}");
                    contextBuilder.AppendLine();
                }

                // Add summary statistics
                contextBuilder.AppendLine("Summary Statistics:");
                contextBuilder.AppendLine($"- Total Available Products: {availableProducts.Count}");
                contextBuilder.AppendLine($"- Price Range: ${availableProducts.Min(p => p.BasePrice):F2} - ${availableProducts.Max(p => p.BasePrice):F2}");
                contextBuilder.AppendLine($"- Average Price: ${availableProducts.Average(p => p.BasePrice):F2}");

                var spiceLevels = availableProducts.Where(p => !string.IsNullOrEmpty(p.SpiceLevel))
                    .GroupBy(p => p.SpiceLevel)
                    .Select(g => $"{g.Key} ({g.Count()})")
                    .ToList();

                if (spiceLevels.Any())
                {
                    contextBuilder.AppendLine($"- Available Spice Levels: {string.Join(", ", spiceLevels)}");
                }

                return contextBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating product context");
                return "Unable to retrieve current product information.";
            }
        }

        private string CreateSystemPrompt(string productContext)
        {
            return $@"You are a helpful restaurant/food ordering assistant. Your role is to help customers choose products based on our current menu and inventory.

{productContext}

Instructions:
1. Always base your recommendations on the available products listed above
2. When asked about prices, refer to the exact prices shown
3. For questions about cheapest items, recommend the products with the lowest prices
4. For questions about spice levels, use the spice level information provided
5. Be conversational and helpful
6. If asked about items not in our database, politely explain they're not currently available
7. Always mention specific product names and prices when making recommendations
8. Keep responses concise but informative
9. If recommending multiple items, limit to 3-5 suggestions maximum

Response Format:
- Provide a natural, conversational response
- Include specific product names and prices
- If recommending products, format them clearly
- Be friendly and helpful in tone";
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            var requestBody = new GeminiRequest
            {
                contents = new List<GeminiContent>
                {
                    new GeminiContent
                    {
                        parts = new List<GeminiPart>
                        {
                            new GeminiPart { text = prompt }
                        }
                    }
                },
                generationConfig = new GeminiGenerationConfig
                {
                    temperature = 0.7,
                    maxOutputTokens = 1000,
                    topP = 0.8,
                    topK = 10
                }
            };

            var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUri = $"{_geminiEndpoint}?key={_geminiApiKey}";
            var response = await _httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API call failed: {StatusCode}, {Content}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Gemini API call failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return geminiResponse?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text ??
                   "I apologize, but I couldn't generate a proper response. Please try again.";
        }

        private async Task<(string response, List<ProductDto>? suggestedProducts, bool isProductQuery)>
            ParseGeminiResponseAsync(string geminiResponse, string userMessage)
        {
            // Determine if this is a product-related query
            var productKeywords = new[] { "order", "buy", "recommend", "suggest", "price", "cheap", "expensive", "spicy", "mild", "food", "dish", "menu" };
            var isProductQuery = productKeywords.Any(keyword =>
                userMessage.ToLower().Contains(keyword));

            List<ProductDto>? suggestedProducts = null;

            if (isProductQuery)
            {
                // Try to extract product recommendations from the response
                suggestedProducts = await ExtractSuggestedProductsAsync(geminiResponse);
            }

            return (geminiResponse, suggestedProducts, isProductQuery);
        }

        private async Task<List<ProductDto>?> ExtractSuggestedProductsAsync(string response)
        {
            try
            {
                var allProducts = await _unitOfWork.Products.FindAsync(p => p.IsAvailable == true);
                var suggestedProducts = new List<Product>();

                // Simple approach: look for product names mentioned in the response
                foreach (var product in allProducts)
                {
                    if (response.Contains(product.ProductName, StringComparison.OrdinalIgnoreCase))
                    {
                        suggestedProducts.Add(product);
                    }
                }

                if (suggestedProducts.Any())
                {
                    var productDtos = _mapper.Map<List<ProductDto>>(suggestedProducts.Take(5).ToList());
                    return productDtos;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting suggested products from response");
                return null;
            }
        }
    }
}
