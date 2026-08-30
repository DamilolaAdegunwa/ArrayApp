using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.NotificationAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(ILogger<NotificationService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationDto> CreateNotificationAsync(NotificationCreateDto notificationCreateDto)
    {
        _logger.LogInformation("Creating notification: {Title}", notificationCreateDto.Title);
        var notification = new Notification
        {
            Title = notificationCreateDto.Title,
            Body = notificationCreateDto.Body,
            Type = notificationCreateDto.Type,
            Importance = notificationCreateDto.Importance,
            Status = "unread",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.NotificationBaseRepository.AddAsync(notification);
        return MapToDto(saved);
    }

    public async Task<NotificationDto> GetNotificationByIdAsync(int notificationId)
    {
        var notification = await _unitOfWork.NotificationBaseRepository.GetByIdAsync(notificationId);
        return notification != null ? MapToDto(notification) : new NotificationDto();
    }

    public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
    {
        var notifications = await _unitOfWork.NotificationBaseRepository.ListAsync();
        return notifications.Select(MapToDto);
    }

    public async Task UpdateNotificationAsync(int notificationId, NotificationUpdateDto notificationUpdateDto)
    {
        var notification = await _unitOfWork.NotificationBaseRepository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            if (!string.IsNullOrWhiteSpace(notificationUpdateDto.Title))
                notification.Title = notificationUpdateDto.Title;
            if (!string.IsNullOrWhiteSpace(notificationUpdateDto.Body))
                notification.Body = notificationUpdateDto.Body;
            if (!string.IsNullOrWhiteSpace(notificationUpdateDto.Status))
                notification.Status = notificationUpdateDto.Status;

            await _unitOfWork.NotificationBaseRepository.UpdateAsync(notification);
        }
    }

    public async Task DeleteNotificationAsync(int notificationId)
    {
        var notification = await _unitOfWork.NotificationBaseRepository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            await _unitOfWork.NotificationBaseRepository.DeleteAsync(notification);
        }
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserAsync(int userId)
    {
        var notifications = await _unitOfWork.NotificationBaseRepository.ListAsync();
        return notifications.Select(MapToDto);
    }

    public async Task<int> GetUnreadNotificationCountAsync(int userId)
    {
        var notifications = await _unitOfWork.NotificationBaseRepository.ListAsync();
        return notifications.Count(n => n.Status == "unread");
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _unitOfWork.NotificationBaseRepository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.Status = "read";
            await _unitOfWork.NotificationBaseRepository.UpdateAsync(notification);
        }
    }

    private static NotificationDto MapToDto(Notification n) => new NotificationDto
    {
        Id = n.Id,
        Title = n.Title,
        Body = n.Body,
        Type = n.Type,
        Importance = n.Importance,
        Status = n.Status,
        CreatedAt = n.CreatedAt
    };
}
