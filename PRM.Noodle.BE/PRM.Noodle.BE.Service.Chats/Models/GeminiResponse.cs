using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class GeminiResponse
    {
        public List<GeminiCandidate> candidates { get; set; } = new();
    }
}
