using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IChatService
{
    Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto);
    Task<ChatDto> GetChatByIdAsync(int chatId);
    Task<IEnumerable<ChatDto>> GetAllChatsAsync();
}
