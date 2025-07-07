using PRM.Noodle.BE.Service.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class ChatResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<ProductDto>? SuggestedProducts { get; set; }
        public bool IsProductQuery { get; set; }
    }
}
