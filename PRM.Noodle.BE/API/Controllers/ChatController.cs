using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.AIChat;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
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
                    return BadRequest(ModelState);

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
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });

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
                return Ok(new { context, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product context");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving product context."
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
    }
}
