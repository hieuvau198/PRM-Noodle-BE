using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class GeminiRequest
    {
        public List<GeminiContent> contents { get; set; } = new();
        public GeminiGenerationConfig generationConfig { get; set; } = new();
    }
}
