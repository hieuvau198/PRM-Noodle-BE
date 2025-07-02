using Services.DTOs.AIChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponseDto> ProcessChatMessageAsync(ChatRequestDto request);
        Task<string> GenerateProductContextAsync();
    }
}
