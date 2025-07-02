using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.AIChat
{
    public class GeminiRequest
    {
        public List<GeminiContent> contents { get; set; } = new();
        public GeminiGenerationConfig generationConfig { get; set; } = new();
    }

    public class GeminiContent
    {
        public List<GeminiPart> parts { get; set; } = new();
    }

    public class GeminiPart
    {
        public string text { get; set; } = string.Empty;
    }

    public class GeminiGenerationConfig
    {
        public double temperature { get; set; } = 0.7;
        public int maxOutputTokens { get; set; } = 1000;
        public double topP { get; set; } = 0.8;
        public int topK { get; set; } = 10;
    }

    public class GeminiResponse
    {
        public List<GeminiCandidate> candidates { get; set; } = new();
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; } = new();
        public string finishReason { get; set; } = string.Empty;
    }
}
