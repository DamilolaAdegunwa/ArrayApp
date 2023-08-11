using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(CommentCreateDto commentCreateDto);
    Task<CommentDto> GetCommentByIdAsync(int commentId);
    Task<IEnumerable<CommentDto>> GetCommentsByIdeaIdAsync(int ideaId);

}
