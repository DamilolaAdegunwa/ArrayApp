using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.SubscriptionAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ILogger<SubscriptionService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(ILogger<SubscriptionService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreateDto)
    {
        _logger.LogInformation("Creating subscription for user: {UserId}, idea: {IdeaId}", subscriptionCreateDto.UserId, subscriptionCreateDto.IdeaId);
        var sub = new Subscription
        {
            Price = subscriptionCreateDto.Price,
            Status = SubscriptionStatus.Active,
            StartDate = DateTimeOffset.UtcNow,
            ExpirationDate = DateTimeOffset.UtcNow.AddMonths(1)
        };

        var saved = await _unitOfWork.SubscriptionBaseRepository.AddAsync(sub);
        return MapToDto(saved, subscriptionCreateDto.UserId, subscriptionCreateDto.IdeaId);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByUserAsync(int userId)
    {
        var subs = await _unitOfWork.SubscriptionBaseRepository.ListAsync();
        return subs.Select(s => MapToDto(s, userId, 0));
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByIdeaAsync(int ideaId)
    {
        var subs = await _unitOfWork.SubscriptionBaseRepository.ListAsync();
        return subs.Select(s => MapToDto(s, 0, ideaId));
    }

    public async Task<bool> IsUserSubscribedAsync(int userId, int ideaId)
    {
        var subs = await _unitOfWork.SubscriptionBaseRepository.ListAsync();
        return subs.Any(s => s.Status == SubscriptionStatus.Active);
    }

    public async Task UnsubscribeAsync(int userId, int ideaId)
    {
        var subs = await _unitOfWork.SubscriptionBaseRepository.ListAsync();
        var active = subs.FirstOrDefault(s => s.Status == SubscriptionStatus.Active);
        if (active != null)
        {
            active.Status = SubscriptionStatus.Cancelled;
            await _unitOfWork.SubscriptionBaseRepository.UpdateAsync(active);
        }
    }

    private static SubscriptionDto MapToDto(Subscription s, int userId, int ideaId) => new SubscriptionDto
    {
        Id = s.Id,
        UserId = userId,
        IdeaId = ideaId,
        Price = s.Price,
        Status = s.Status.ToString(),
        StartDate = s.StartDate,
        ExpirationDate = s.ExpirationDate
    };
}
