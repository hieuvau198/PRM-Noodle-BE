using PRM.Noodle.BE.Service.Chats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Chats.Services
{
    public interface IChatService
    {
        Task<ChatResponseDto> ProcessChatMessageAsync(ChatRequestDto request);
        Task<string> GenerateProductContextAsync();
    }
}
