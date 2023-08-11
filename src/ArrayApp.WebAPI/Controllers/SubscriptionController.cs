using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSubscription(SubscriptionCreateDto subscriptionCreateDto)
    {
        var createdSubscription = await _subscriptionService.CreateSubscriptionAsync(subscriptionCreateDto);
        return Ok(new ApiResponse<SubscriptionDto>
        {
            Code = SystemCodes.Successful,
            Data = createdSubscription,
            Description = "Subscription created successfully",
        });
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetSubscriptionsByUser(int userId)
    {
        var subscriptions = await _subscriptionService.GetSubscriptionsByUserAsync(userId);
        return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>
        {
            Code = SystemCodes.Successful,
            Data = subscriptions,
            Description = "Subscriptions retrieved successfully",
        });
    }

    [HttpGet("idea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetSubscriptionsByIdea(int ideaId)
    {
        var subscriptions = await _subscriptionService.GetSubscriptionsByIdeaAsync(ideaId);
        return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>
        {
            Code = SystemCodes.Successful,
            Data = subscriptions,
            Description = "Subscriptions retrieved successfully",
        });
    }

    // Add other endpoints based on the remaining service methods
}
