using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.CommentAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(ILogger<CommentService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentDto> CreateCommentAsync(CommentCreateDto commentCreateDto)
    {
        _logger.LogInformation("Creating comment for idea: {IdeaId}", commentCreateDto.IdeaId);
        var comment = new Comment
        {
            Text = commentCreateDto.Text,
            Rating = commentCreateDto.Rating,
            Status = commentCreateDto.Status,
            Content = commentCreateDto.IdeaId > 0 ? $"Idea:{commentCreateDto.IdeaId}" : commentCreateDto.Content,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.CommentBaseRepository.AddAsync(comment);
        return MapToDto(saved);
    }

    public async Task<CommentDto> GetCommentByIdAsync(int commentId)
    {
        var comment = await _unitOfWork.CommentBaseRepository.GetByIdAsync(commentId);
        return comment != null ? MapToDto(comment) : new CommentDto();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByIdeaIdAsync(int ideaId)
    {
        var comments = await _unitOfWork.CommentBaseRepository.ListAsync();
        return comments
            .Where(c => c.Content != null && (c.Content == $"Idea:{ideaId}" || c.Content.Contains(ideaId.ToString())))
            .Select(MapToDto);
    }

    private static CommentDto MapToDto(Comment c) => new CommentDto
    {
        Id = c.Id,
        Text = c.Text,
        Rating = c.Rating,
        Status = c.Status,
        Content = c.Content,
        CreatedAt = c.CreatedAt
    };
}
