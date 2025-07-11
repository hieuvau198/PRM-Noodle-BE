using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRM.Noodle.BE.Service.Chats.Models;
using PRM.Noodle.BE.Service.Chats.Services;
using System;
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request == null || string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { message = "Message cannot be empty" });
                }

                var response = await _chatService.ProcessChatMessageAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid chat request: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your message. Please try again later.",
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
            try
            {
                var context = await _chatService.GenerateProductContextAsync();
                return Ok(new
                {
                    context,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product context");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving product context.",
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
            return Ok(new
            {
                status = "healthy",
                service = "chat",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Debug endpoint to check configuration
        /// </summary>
        /// <returns>Configuration debug info</returns>
        [HttpGet("debug/config")]
        public ActionResult<object> GetConfigDebug()
        {
            try
            {
                var apiKey = _configuration["GeminiAPI:ApiKey"];
                var endpoint = _configuration["GeminiAPI:Endpoint"];

                var result = new
                {
                    timestamp = DateTime.UtcNow,
                    configuration = new
                    {
                        hasGeminiApiKey = !string.IsNullOrEmpty(apiKey),
                        apiKeyLength = apiKey?.Length ?? 0,
                        endpoint = endpoint ?? "null"
                    },
                    environment = new
                    {
                        machineName = Environment.MachineName,
                        environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown"
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in configuration debug");
                return StatusCode(500, new
                {
                    message = "Error retrieving configuration debug info",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}