using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(NotificationCreateDto notificationCreateDto);
    Task<NotificationDto> GetNotificationByIdAsync(int notificationId);
    Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
    Task UpdateNotificationAsync(int notificationId, NotificationUpdateDto notificationUpdateDto);
    Task DeleteNotificationAsync(int notificationId);
    Task<IEnumerable<NotificationDto>> GetNotificationsByUserAsync(int userId);
    Task<int> GetUnreadNotificationCountAsync(int userId);
    Task MarkNotificationAsReadAsync(int notificationId);
}
