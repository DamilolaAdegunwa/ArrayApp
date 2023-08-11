using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface ITagService
{
    Task<TagDto> CreateTagAsync(TagCreateDto tagCreateDto);
    Task<IEnumerable<TagDto>> GetAllTagsAsync();
    Task<IEnumerable<TagDto>> GetTagsByIdeaAsync(int ideaId);
}
