using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.AppAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class AppService : IAppService
{
    private readonly ILogger<AppService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AppService(ILogger<AppService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppDto> CreateAppAsync(AppCreateDto appCreateDto)
    {
        _logger.LogInformation("Creating app: {Name}", appCreateDto.Name);
        var app = new App
        {
            Name = appCreateDto.Name,
            Description = appCreateDto.Description,
            Price = appCreateDto.Price,
            Version = appCreateDto.Version,
            ReleaseNotes = appCreateDto.ReleaseNotes,
            Rating = 5.0,
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.AppBaseRepository.AddAsync(app);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<AppDto>> GetAllAppsAsync()
    {
        var apps = await _unitOfWork.AppBaseRepository.ListAsync();
        return apps.Select(MapToDto);
    }

    public async Task<AppDto> GetAppByIdAsync(int appId)
    {
        var app = await _unitOfWork.AppBaseRepository.GetByIdAsync(appId);
        return app != null ? MapToDto(app) : new AppDto();
    }

    private static AppDto MapToDto(App a) => new AppDto
    {
        Id = a.Id,
        Name = a.Name,
        Description = a.Description,
        Price = a.Price,
        Rating = a.Rating,
        Version = a.Version,
        ReleaseNotes = a.ReleaseNotes,
        CreatedAt = a.CreatedAt,
        ModifiedAt = a.ModifiedAt
    };
}
