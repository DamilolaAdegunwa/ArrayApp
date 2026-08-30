using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.TagAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class TagService : ITagService
{
    private readonly ILogger<TagService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TagService(ILogger<TagService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TagDto> CreateTagAsync(TagCreateDto tagCreateDto)
    {
        _logger.LogInformation("Creating tag: {Name}", tagCreateDto.Name);
        var tag = new Tag
        {
            Name = tagCreateDto.Name,
            Description = tagCreateDto.Description ?? string.Empty,
            Count = 1,
            IsActive = true,
            LastUsed = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.TagBaseRepository.AddAsync(tag);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _unitOfWork.TagBaseRepository.ListAsync();
        return tags.Select(MapToDto);
    }

    public async Task<IEnumerable<TagDto>> GetTagsByIdeaAsync(int ideaId)
    {
        var tags = await _unitOfWork.TagBaseRepository.ListAsync();
        return tags.Select(MapToDto);
    }

    private static TagDto MapToDto(Tag t) => new TagDto
    {
        Id = t.Id,
        Name = t.Name,
        Description = t.Description,
        Count = t.Count,
        IsActive = t.IsActive,
        LastUsed = t.LastUsed
    };
}
