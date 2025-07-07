using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
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

            // Enhanced configuration logging
            _logger.LogInformation("=== ChatService Configuration Loading ===");

            var apiKey = _configuration["GeminiAPI:ApiKey"];
            var endpoint = _configuration["GeminiAPI:Endpoint"];

            _logger.LogInformation("GeminiAPI:ApiKey configured: {HasApiKey}", !string.IsNullOrEmpty(apiKey));
            _logger.LogInformation("GeminiAPI:ApiKey length: {KeyLength}", apiKey?.Length ?? 0);
            _logger.LogInformation("GeminiAPI:ApiKey starts with: {KeyPrefix}",
                !string.IsNullOrEmpty(apiKey) ? apiKey.Substring(0, Math.Min(10, apiKey.Length)) + "..." : "null");
            _logger.LogInformation("GeminiAPI:Endpoint: {Endpoint}", endpoint ?? "null");

            // Log all configuration keys that start with "Gemini" for debugging
            var geminiKeys = _configuration.AsEnumerable()
                .Where(kvp => kvp.Key?.StartsWith("Gemini", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            _logger.LogInformation("All Gemini-related configuration keys found: {Count}", geminiKeys.Count);
            foreach (var key in geminiKeys)
            {
                _logger.LogInformation("Config Key: '{Key}' = '{Value}'",
                    key.Key,
                    key.Key.Contains("ApiKey", StringComparison.OrdinalIgnoreCase)
                        ? (string.IsNullOrEmpty(key.Value) ? "null" : $"{key.Value.Substring(0, Math.Min(10, key.Value.Length))}...")
                        : key.Value);
            }

            _geminiApiKey = apiKey ?? throw new InvalidOperationException("Gemini API key not configured");
            _geminiEndpoint = endpoint ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent";

            _logger.LogInformation("ChatService initialized successfully");
            _logger.LogInformation("=== End Configuration Loading ===");
        }

        public async Task<ChatResponseDto> ProcessChatMessageAsync(ChatRequestDto request)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("[{CorrelationId}] Processing chat message: '{Message}'", correlationId, request.Message);

            try
            {
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
                _logger.LogInformation("[{CorrelationId}] Using session ID: {SessionId}", correlationId, sessionId);

                // Generate product context
                _logger.LogInformation("[{CorrelationId}] Generating product context...", correlationId);
                var productContext = await GenerateProductContextAsync();
                _logger.LogInformation("[{CorrelationId}] Product context generated. Length: {Length} characters",
                    correlationId, productContext.Length);

                // Create system prompt
                _logger.LogInformation("[{CorrelationId}] Creating system prompt...", correlationId);
                var systemPrompt = CreateSystemPrompt(productContext);
                _logger.LogInformation("[{CorrelationId}] System prompt created. Length: {Length} characters",
                    correlationId, systemPrompt.Length);

                // Build the complete prompt
                var completePrompt = $"{systemPrompt}\n\nUser Question: {request.Message}";
                _logger.LogInformation("[{CorrelationId}] Complete prompt length: {Length} characters",
                    correlationId, completePrompt.Length);

                // Call Gemini API
                _logger.LogInformation("[{CorrelationId}] Calling Gemini API...", correlationId);
                var geminiResponse = await CallGeminiApiAsync(completePrompt, correlationId);
                _logger.LogInformation("[{CorrelationId}] Gemini API call completed successfully", correlationId);

                // Parse response and extract suggested products if any
                _logger.LogInformation("[{CorrelationId}] Parsing Gemini response...", correlationId);
                var (response, suggestedProducts, isProductQuery) = await ParseGeminiResponseAsync(geminiResponse, request.Message);
                _logger.LogInformation("[{CorrelationId}] Response parsed. IsProductQuery: {IsProductQuery}, SuggestedProducts: {Count}",
                    correlationId, isProductQuery, suggestedProducts?.Count ?? 0);

                var result = new ChatResponseDto
                {
                    Response = response,
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow,
                    SuggestedProducts = suggestedProducts,
                    IsProductQuery = isProductQuery
                };

                _logger.LogInformation("[{CorrelationId}] Chat processing completed successfully", correlationId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] Error processing chat message: {Message}", correlationId, request.Message);
                return new ChatResponseDto
                {
                    Response = "I apologize, but I'm having trouble processing your request right now. Please try again later."
                        + $"\n[{correlationId}] " + ex.Message + "\n" + ex.StackTrace + "\n" + ex.ToString(),
                    SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    IsProductQuery = false
                };
            }
        }

        public async Task<string> GenerateProductContextAsync()
        {
            _logger.LogInformation("Generating product context...");
            try
            {
                var products = await _unitOfWork.Products.FindAsync(p => p.IsAvailable == true);
                var availableProducts = products.ToList();
                _logger.LogInformation("Found {Count} available products", availableProducts.Count);

                if (!availableProducts.Any())
                {
                    _logger.LogWarning("No available products found in database");
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

                var context = contextBuilder.ToString();
                _logger.LogInformation("Product context generated successfully. Length: {Length} characters", context.Length);
                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating product context");
                return "Unable to retrieve current product information.";
            }
        }

        private string CreateSystemPrompt(string productContext)
        {
            _logger.LogDebug("Creating system prompt with product context length: {Length}", productContext.Length);

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

        private async Task<string> CallGeminiApiAsync(string prompt, string correlationId = null)
        {
            correlationId = correlationId ?? Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("[{CorrelationId}] === Gemini API Call Starting ===", correlationId);
                _logger.LogInformation("[{CorrelationId}] API Key configured: {HasKey}", correlationId, !string.IsNullOrEmpty(_geminiApiKey));
                _logger.LogInformation("[{CorrelationId}] API Key length: {Length}", correlationId, _geminiApiKey?.Length ?? 0);
                _logger.LogInformation("[{CorrelationId}] Endpoint: {Endpoint}", correlationId, _geminiEndpoint);
                _logger.LogInformation("[{CorrelationId}] Prompt length: {Length} characters", correlationId, prompt.Length);

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

                _logger.LogInformation("[{CorrelationId}] Request JSON length: {Length} characters", correlationId, json.Length);
                _logger.LogDebug("[{CorrelationId}] Request JSON: {Json}", correlationId, json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var requestUri = $"{_geminiEndpoint}?key={_geminiApiKey}";

                _logger.LogInformation("[{CorrelationId}] Making HTTP POST request to: {Uri}",
                    correlationId, $"{_geminiEndpoint}?key=***");

                var response = await _httpClient.PostAsync(requestUri, content);

                _logger.LogInformation("[{CorrelationId}] HTTP Response Status: {StatusCode}", correlationId, response.StatusCode);
                _logger.LogInformation("[{CorrelationId}] HTTP Response Headers: {Headers}",
                    correlationId, string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(";", h.Value)}")));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("[{CorrelationId}] Gemini API call failed!", correlationId);
                    _logger.LogError("[{CorrelationId}] Status Code: {StatusCode}", correlationId, response.StatusCode);
                    _logger.LogError("[{CorrelationId}] Error Content: {Content}", correlationId, errorContent);
                    _logger.LogError("[{CorrelationId}] Request URI (masked): {Uri}", correlationId, $"{_geminiEndpoint}?key=***");
                    _logger.LogError("[{CorrelationId}] Full Request Body: {RequestBody}", correlationId, json);

                    throw new HttpRequestException($"[{correlationId}] Gemini API call failed: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("[{CorrelationId}] Response content length: {Length} characters", correlationId, responseContent.Length);
                _logger.LogDebug("[{CorrelationId}] Response content: {Content}", correlationId, responseContent);

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var extractedText = geminiResponse?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text;
                _logger.LogInformation("[{CorrelationId}] Extracted text length: {Length} characters",
                    correlationId, extractedText?.Length ?? 0);

                _logger.LogInformation("[{CorrelationId}] === Gemini API Call Completed Successfully ===", correlationId);

                return extractedText ?? "I apologize, but I couldn't generate a proper response. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] Exception in CallGeminiApiAsync", correlationId);
                _logger.LogError("[{CorrelationId}] Exception Type: {Type}", correlationId, ex.GetType().Name);
                _logger.LogError("[{CorrelationId}] Exception Message: {Message}", correlationId, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError("[{CorrelationId}] Inner Exception: {InnerException}", correlationId, ex.InnerException.ToString());
                }
                throw;
            }
        }

        private async Task<(string response, List<ProductDto>? suggestedProducts, bool isProductQuery)>
            ParseGeminiResponseAsync(string geminiResponse, string userMessage)
        {
            _logger.LogInformation("Parsing Gemini response and user message for product queries");

            // Determine if this is a product-related query
            var productKeywords = new[] { "order", "buy", "recommend", "suggest", "price", "cheap", "expensive", "spicy", "mild", "food", "dish", "menu" };
            var isProductQuery = productKeywords.Any(keyword =>
                userMessage.ToLower().Contains(keyword));

            _logger.LogInformation("Product query detection: {IsProductQuery} (matched keywords: {Keywords})",
                isProductQuery,
                string.Join(", ", productKeywords.Where(k => userMessage.ToLower().Contains(k))));

            List<ProductDto>? suggestedProducts = null;

            if (isProductQuery)
            {
                _logger.LogInformation("Extracting suggested products from response...");
                // Try to extract product recommendations from the response
                suggestedProducts = await ExtractSuggestedProductsAsync(geminiResponse);
                _logger.LogInformation("Extracted {Count} suggested products", suggestedProducts?.Count ?? 0);
            }

            return (geminiResponse, suggestedProducts, isProductQuery);
        }

        private async Task<List<ProductDto>?> ExtractSuggestedProductsAsync(string response)
        {
            _logger.LogInformation("Extracting suggested products from response...");

            try
            {
                var allProducts = await _unitOfWork.Products.FindAsync(p => p.IsAvailable == true);
                var suggestedProducts = new List<Product>();

                _logger.LogInformation("Checking {Count} available products against response", allProducts.Count());

                // Simple approach: look for product names mentioned in the response
                foreach (var product in allProducts)
                {
                    if (response.Contains(product.ProductName, StringComparison.OrdinalIgnoreCase))
                    {
                        suggestedProducts.Add(product);
                        _logger.LogInformation("Found matching product: {ProductName}", product.ProductName);
                    }
                }

                if (suggestedProducts.Any())
                {
                    var productDtos = _mapper.Map<List<ProductDto>>(suggestedProducts.Take(5).ToList());
                    _logger.LogInformation("Returning {Count} suggested products", productDtos.Count);
                    return productDtos;
                }

                _logger.LogInformation("No matching products found in response");
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