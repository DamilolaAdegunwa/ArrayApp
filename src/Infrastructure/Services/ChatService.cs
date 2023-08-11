using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;
public class ChatService : IChatService
{
    public Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ChatDto>> GetAllChatsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ChatDto> GetChatByIdAsync(int chatId)
    {
        throw new NotImplementedException();
    }
}
