using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class ChatRequestDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters")]
        public string Message { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SessionId { get; set; }
    }
}
