using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Models
{
    public class GeminiContent
    {
        public List<GeminiPart> parts { get; set; } = new();
    }
}
