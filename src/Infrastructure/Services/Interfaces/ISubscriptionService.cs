using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface ISubscriptionService
{
    Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreateDto);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByUserAsync(int userId);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByIdeaAsync(int ideaId);
    Task<bool> IsUserSubscribedAsync(int userId, int ideaId);
    Task UnsubscribeAsync(int userId, int ideaId);
}
