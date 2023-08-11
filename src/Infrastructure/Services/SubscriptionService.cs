using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    public Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByIdeaAsync(int ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByUserAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserSubscribedAsync(int userId, int ideaId)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(int userId, int ideaId)
    {
        throw new NotImplementedException();
    }
}
