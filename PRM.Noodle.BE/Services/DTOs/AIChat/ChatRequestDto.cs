//using Services.DTOs.Product;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Services.DTOs.AIChat
//{
//    public class ChatRequestDto
//    {
//        [Required]
//        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters")]
//        public string Message { get; set; } = string.Empty;

//        [StringLength(100)]
//        public string? SessionId { get; set; }
//    }
//    public class ChatResponseDto
//    {
//        public string Response { get; set; } = string.Empty;
//        public string SessionId { get; set; } = string.Empty;
//        public DateTime Timestamp { get; set; }
//        public List<ProductDto>? SuggestedProducts { get; set; }
//        public bool IsProductQuery { get; set; }
//    }
//}
