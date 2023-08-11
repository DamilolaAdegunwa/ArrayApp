using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;
public class CommentService : ICommentService
{
    public Task<CommentDto> CreateCommentAsync(CommentCreateDto commentCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<CommentDto> GetCommentByIdAsync(int commentId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CommentDto>> GetCommentsByIdeaIdAsync(int ideaId)
    {
        throw new NotImplementedException();
    }
}
