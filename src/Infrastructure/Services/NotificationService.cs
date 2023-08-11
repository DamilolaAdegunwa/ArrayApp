using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;
public class NotificationService : INotificationService
{
    public Task<NotificationDto> CreateNotificationAsync(NotificationCreateDto notificationCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteNotificationAsync(int notificationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<NotificationDto> GetNotificationByIdAsync(int notificationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<NotificationDto>> GetNotificationsByUserAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetUnreadNotificationCountAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task MarkNotificationAsReadAsync(int notificationId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateNotificationAsync(int notificationId, NotificationUpdateDto notificationUpdateDto)
    {
        throw new NotImplementedException();
    }
}
