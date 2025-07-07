using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class GeminiCandidate
    {
        public GeminiContent content { get; set; } = new();
        public string finishReason { get; set; } = string.Empty;
    }
}
