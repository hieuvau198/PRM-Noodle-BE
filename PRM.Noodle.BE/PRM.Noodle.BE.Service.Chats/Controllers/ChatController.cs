using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRM.Noodle.BE.Service.Chats.Models;
using PRM.Noodle.BE.Service.Chats.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;

        public ChatController(IChatService chatService, ILogger<ChatController> logger, IConfiguration configuration)
        {
            _chatService = chatService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Process a chat message and get AI response based on available products
        /// </summary>
        /// <param name="request">Chat request containing user message</param>
        /// <returns>AI response with product recommendations if applicable</returns>
        [HttpPost("message")]
        public async Task<ActionResult<ChatResponseDto>> ProcessMessage([FromBody] ChatRequestDto request)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("[{RequestId}] === Chat Message Request Started ===", requestId);
            _logger.LogInformation("[{RequestId}] Request IP: {IP}", requestId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            _logger.LogInformation("[{RequestId}] User Message: '{Message}'", requestId, request?.Message ?? "null");
            _logger.LogInformation("[{RequestId}] Session ID: {SessionId}", requestId, request?.SessionId ?? "null");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("[{RequestId}] Invalid model state: {Errors}", requestId,
                        string.Join(", ", ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))));
                    return BadRequest(ModelState);
                }

                if (request == null || string.IsNullOrWhiteSpace(request.Message))
                {
                    _logger.LogWarning("[{RequestId}] Empty or null request message", requestId);
                    return BadRequest(new { message = "Message cannot be empty" });
                }

                _logger.LogInformation("[{RequestId}] Calling chat service...", requestId);
                var response = await _chatService.ProcessChatMessageAsync(request);

                _logger.LogInformation("[{RequestId}] Chat service completed. Response length: {Length}",
                    requestId, response?.Response?.Length ?? 0);
                _logger.LogInformation("[{RequestId}] === Chat Message Request Completed Successfully ===", requestId);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("[{RequestId}] Invalid chat request: {Message}", requestId, ex.Message);
                return BadRequest(new { message = ex.Message, requestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RequestId}] Error processing chat message", requestId);
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your message. Please try again later.",
                    requestId,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get current product context for debugging purposes
        /// </summary>
        /// <returns>Current product context used by AI</returns>
        [HttpGet("context")]
        public async Task<ActionResult<object>> GetProductContext()
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("[{RequestId}] Product context requested", requestId);

            try
            {
                var context = await _chatService.GenerateProductContextAsync();
                _logger.LogInformation("[{RequestId}] Product context generated successfully. Length: {Length}",
                    requestId, context?.Length ?? 0);

                return Ok(new
                {
                    context,
                    timestamp = DateTime.UtcNow,
                    requestId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RequestId}] Error retrieving product context", requestId);
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving product context.",
                    requestId,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Health check endpoint for chat service
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public ActionResult<object> HealthCheck()
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("[{RequestId}] Health check requested", requestId);

            return Ok(new
            {
                status = "healthy",
                service = "chat",
                timestamp = DateTime.UtcNow,
                requestId
            });
        }

        /// <summary>
        /// Debug endpoint to check configuration
        /// </summary>
        /// <returns>Configuration debug info</returns>
        [HttpGet("debug/config")]
        public ActionResult<object> GetConfigDebug()
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("[{RequestId}] Configuration debug requested", requestId);

            try
            {
                var apiKey = _configuration["GeminiAPI:ApiKey"];
                var endpoint = _configuration["GeminiAPI:Endpoint"];

                // Get all configuration keys that contain "Gemini"
                var geminiKeys = _configuration.AsEnumerable()
                    .Where(kvp => kvp.Key?.Contains("Gemini", StringComparison.OrdinalIgnoreCase) == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Key.Contains("ApiKey", StringComparison.OrdinalIgnoreCase)
                            ? string.IsNullOrEmpty(kvp.Value) ? "null" : $"***{kvp.Value.Substring(Math.Max(0, kvp.Value.Length - 4))}"
                            : kvp.Value
                    );

                var result = new
                {
                    timestamp = DateTime.UtcNow,
                    requestId,
                    configuration = new
                    {
                        hasGeminiApiKey = !string.IsNullOrEmpty(apiKey),
                        apiKeyLength = apiKey?.Length ?? 0,
                        apiKeyLastFourChars = !string.IsNullOrEmpty(apiKey) && apiKey.Length >= 4
                            ? apiKey.Substring(apiKey.Length - 4)
                            : "N/A",
                        endpoint = endpoint ?? "null",
                        allGeminiKeys = geminiKeys
                    },
                    environment = new
                    {
                        machineName = Environment.MachineName,
                        environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown",
                        isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"
                    }
                };

                _logger.LogInformation("[{RequestId}] Configuration debug completed", requestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RequestId}] Error in configuration debug", requestId);
                return StatusCode(500, new
                {
                    message = "Error retrieving configuration debug info",
                    requestId,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
