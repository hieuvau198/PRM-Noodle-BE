using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class GeminiGenerationConfig
    {
        public double temperature { get; set; } = 0.7;
        public int maxOutputTokens { get; set; } = 1000;
        public double topP { get; set; } = 0.8;
        public int topK { get; set; } = 10;
    }
}
