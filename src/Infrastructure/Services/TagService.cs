using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
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
    public Task<TagDto> CreateTagAsync(TagCreateDto tagCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TagDto>> GetTagsByIdeaAsync(int ideaId)
    {
        throw new NotImplementedException();
    }
}
