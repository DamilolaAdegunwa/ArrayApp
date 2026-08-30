using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.AdvertAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class AdvertService : IAdvertService
{
    private readonly ILogger<AdvertService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AdvertService(ILogger<AdvertService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdvertDto> CreateAdvertAsync(AdvertCreateDto advertCreateDto)
    {
        _logger.LogInformation("Creating advert: {Title}", advertCreateDto.Title);
        var advert = new Advert
        {
            Title = advertCreateDto.Title ?? string.Empty,
            Description = advertCreateDto.Description ?? string.Empty,
            Price = advertCreateDto.Price,
            Location = advertCreateDto.Location ?? string.Empty,
            IsActive = advertCreateDto.IsActive,
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.AdvertBaseRepository.AddAsync(advert);
        return MapToDto(saved);
    }

    public async Task<AdvertDto> GetAdvertByIdAsync(int advertId)
    {
        var advert = await _unitOfWork.AdvertBaseRepository.GetByIdAsync(advertId);
        return advert != null ? MapToDto(advert) : new AdvertDto();
    }

    public async Task<IEnumerable<AdvertDto>> GetAllAdvertsAsync()
    {
        var adverts = await _unitOfWork.AdvertBaseRepository.ListAsync();
        return adverts.Select(MapToDto);
    }

    private static AdvertDto MapToDto(Advert a) => new AdvertDto
    {
        Title = a.Title,
        Description = a.Description,
        Price = a.Price,
        Location = a.Location,
        IsActive = a.IsActive,
        CreatedAt = a.CreatedAt,
        ModifiedAt = a.ModifiedAt,
        Category = a.Category,
        Images = a.Images
    };
}
