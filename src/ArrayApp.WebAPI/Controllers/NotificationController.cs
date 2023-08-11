using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateNotification(NotificationCreateDto notificationCreateDto)
    {
        var createdNotification = await _notificationService.CreateNotificationAsync(notificationCreateDto);
        return Ok(new ApiResponse<NotificationDto>
        {
            Code = SystemCodes.Successful,
            Data = createdNotification,
            Description = "Notification created successfully",
        });
    }

    [HttpGet("get/{notificationId}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetNotificationById(int notificationId)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
        return Ok(new ApiResponse<NotificationDto>
        {
            Code = SystemCodes.Successful,
            Data = notification,
            Description = "Notification retrieved successfully",
        });
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(new ApiResponse<IEnumerable<NotificationDto>>
        {
            Code = SystemCodes.Successful,
            Data = notifications,
            Description = "All notifications retrieved successfully",
        });
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var notifications = await _notificationService.GetNotificationsByUserAsync(userId);
        return Ok(new ApiResponse<IEnumerable<NotificationDto>>
        {
            Code = SystemCodes.Successful,
            Data = notifications,
            Description = "Notifications for user retrieved successfully",
        });
    }

    // Add other methods based on the remaining service methods
}
