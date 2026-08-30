using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.ChatAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(ILogger<ChatService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto)
    {
        _logger.LogInformation("Creating chat: {Name}", chatCreateDto.Name);
        var chat = new Chat
        {
            Name = chatCreateDto.Name,
            Type = chatCreateDto.Type,
            Status = chatCreateDto.Status,
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.ChatBaseRepository.AddAsync(chat);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<ChatDto>> GetAllChatsAsync()
    {
        var chats = await _unitOfWork.ChatBaseRepository.ListAsync();
        return chats.Select(MapToDto);
    }

    public async Task<ChatDto> GetChatByIdAsync(int chatId)
    {
        var chat = await _unitOfWork.ChatBaseRepository.GetByIdAsync(chatId);
        return chat != null ? MapToDto(chat) : new ChatDto();
    }

    private static ChatDto MapToDto(Chat c) => new ChatDto
    {
        Id = c.Id,
        Name = c.Name,
        Type = c.Type,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        ModifiedAt = c.ModifiedAt
    };
}
