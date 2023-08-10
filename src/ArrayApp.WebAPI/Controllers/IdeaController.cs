using Microsoft.AspNetCore.Mvc;
using System.Net;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using Microsoft.AspNetCore.Authorization;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ArrayApp.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class IdeaController : ApiControllerBase
{
    private readonly IIdeaService _ideaService;
    private readonly ILogger<IdeaController> _logger;

    public IdeaController(IIdeaService ideaService, ILogger<IdeaController> logger)
    {
        _ideaService = ideaService;
        _logger = logger;
    }

    [HttpGet("feed")]
    [ProducesResponseType(typeof(ApiResponse<List<Idea>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFeed()
    {
        try
        {
            // Call the service to get the main home feed
            var result = await _ideaService.GetFeed();

            return Ok(new ApiResponse<List<Idea>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Main home feed retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving main home feed");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("timeline/{username}")]
    [ProducesResponseType(typeof(ApiResponse<UserTimelineModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UserTimeline(string username)
    {
        try
        {
            // Call the service to get the main home feed
            var result = await _ideaService.UserTimeline(username);

            return Ok(new ApiResponse<UserTimelineModel>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving data");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("explore")]
    [ProducesResponseType(typeof(ApiResponse<ExploreModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Explore()
    {
        try
        {
            // Call the service to get trending topics and suggested users
            var result = await _ideaService.GetExplore();

            return Ok(new ApiResponse<ExploreModel>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Explore data retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving explore data");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<SearchModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        try
        {
            // Call the service to search for ideas, users, hashtags, etc.
            var result = await _ideaService.Search(query);

            return Ok(new ApiResponse<SearchModel>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Search results retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while searching");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("notifications")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetNotifications()
    {
        try
        {
            // Call the service to get user notifications
            var result = await _ideaService.GetUserNotifications();

            return Ok(new ApiResponse<NotificationResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User notifications retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving user notifications");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("messages")]
    [ProducesResponseType(typeof(ApiResponse<MessagesResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetMessages()
    {
        try
        {
            // Call the service to get direct messages and conversations
            var result = await _ideaService.GetDirectMessages();

            return Ok(new ApiResponse<MessagesResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Direct messages retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving direct messages");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpGet("profile/{username}")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserProfile(string username)
    {
        try
        {
            // Call the service to get user profile information
            var result = await _ideaService.GetUserProfile(username);

            return Ok(new ApiResponse<UserProfileModel>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User profile retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving user profile");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<IdeaResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateIdea([FromBody] CreateIdeaRequest request)
    {
        try
        {
            // Call the service to create a new idea
            var result = await _ideaService.CreateIdea(request);

            return Ok(new ApiResponse<IdeaResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Idea created successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating a Idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("comment")]
    [ProducesResponseType(typeof(ApiResponse<CommentResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CommentOnIdea([FromBody] CommentRequest request)
    {
        try
        {
            // Call the service to add a comment to an idea
            var result = await _ideaService.CommentOnIdea(request);

            return Ok(new ApiResponse<CommentResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Comment added successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding a comment");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost("like/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> LikeIdea(string ideaId)
    {
        try
        {
            // Call the service to like a idea
            await _ideaService.LikeIdea(ideaId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Idea liked successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while liking a Idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("unlike/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UnlikeIdea(string ideaId)
    {
        try
        {
            // Call the service to unlike a idea
            await _ideaService.UnlikeIdea(ideaId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Idea unliked successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unliking a idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost("share-idea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<ShareIdeaResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ShareIdea(string ideaId)
    {
        try
        {
            // Call the service to reidea a idea
            var result = await _ideaService.ShareIdea(ideaId);

            return Ok(new ApiResponse<ShareIdeaResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Idea shared successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sharing an idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("unshare-idea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UnshareIdea(string ideaId)
    {
        try
        {
            // Call the service to unshare idea
            await _ideaService.UnshareIdea(ideaId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Idea unreideaed successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unreideaing a idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost("follow/{username}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FollowUser(string username)
    {
        try
        {
            // Call the service to follow a user
            await _ideaService.FollowUser(username);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User followed successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while following a user");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("unfollow/{username}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UnfollowUser(string username)
    {
        try
        {
            // Call the service to unfollow a user
            await _ideaService.UnfollowUser(username);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User unfollowed successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unfollowing a user");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost("message")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SendDirectMessage([FromBody] DirectMessageRequest request)
    {
        try
        {
            // Call the service to send a direct message
            await _ideaService.SendDirectMessage(request);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Direct message sent successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending a direct message");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPut("profile/update")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
    {
        try
        {
            // Call the service to update user profile information
            await _ideaService.UpdateUserProfile(request);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User profile updated successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating user profile");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpDelete("idea/{ideaId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteIdea(string ideaId)
    {
        try
        {
            // Call the service to delete a idea
            await _ideaService.DeleteIdea(ideaId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Idea deleted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a idea");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpDelete("comment/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteComment(string commentId)
    {
        try
        {
            // Call the service to delete a comment
            await _ideaService.DeleteComment(commentId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Comment deleted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a comment");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpDelete("message/{messageId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteDirectMessage(string messageId)
    {
        try
        {
            // Call the service to delete a direct message
            await _ideaService.DeleteDirectMessage(messageId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Direct message deleted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a direct message");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpGet("settings")]
    [ProducesResponseType(typeof(ApiResponse<AccountSettings>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserAccountSettings()
    {
        try
        {
            // Call the service to get user account settings
            var result = await _ideaService.GetUserAccountSettings();

            return Ok(new ApiResponse<AccountSettings>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User account settings retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving user account settings");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPut("settings/update")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserAccountSettings([FromBody] UpdateAccountSettingsRequest request)
    {
        try
        {
            // Call the service to update user account settings
            await _ideaService.UpdateUserAccountSettings(request);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User account settings updated successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating user account settings");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<List<Idea>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllIdeas()
    {
        try
        {
            // Call the service to get all ideas
            var result = await _ideaService.GetAllIdeas();

            return Ok(new ApiResponse<List<Idea>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User-created lists retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving user-created lists");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("lists/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<Idea>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetIdeasFromUser(string userId)
    {
        try
        {
            // Call the service to get ideas from a specific user list
            var result = await _ideaService.GetIdeasFromUser(userId);

            return Ok(new ApiResponse<List<Idea>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Ideas from user list retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving ideas from user list");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("user-created-lists")]
    [ProducesResponseType(typeof(ApiResponse<UserCreatedListResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateUserList([FromBody] UserCreatedListRequest request)
    {//UserCreatedList
        try
        {
            // Call the service to create a new user list
            var result = await _ideaService.GetUserCreatedLists(request);

            return Ok(new ApiResponse<UserCreatedListResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User created list successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating a user list");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPut("user-created-lists/update")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserList([FromBody] UpdateUserCreatedListRequest request)
    {
        try
        {
            // Call the service to update a user list
            await _ideaService.UpdateUserCreatedList(request);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User list updated successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating a user list");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpDelete("lists/{listId}/delete")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteUserList(string listId)
    {
        try
        {
            // Call the service to delete a user list
            await _ideaService.DeleteUserCreatedList(listId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "User list deleted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a user list");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("analytics")]
    [ProducesResponseType(typeof(ApiResponse<EngagementAnalytics>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserEngagementAnalytics()
    {
        try
        {
            // Call the service to get user engagement analytics
            var result = await _ideaService.GetUserEngagementAnalytics();

            return Ok(new ApiResponse<EngagementAnalytics>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User engagement analytics retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving user engagement analytics");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpGet("admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    [Authorize(Roles = "Admin")]
    public IActionResult AccessAdminPanel()
    {
        try
        {
            // Your logic to access the administrative panel

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Access granted to the administrative panel",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while accessing the administrative panel");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    #region sample (commented!)
    //[HttpPost]
    //[ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
    //[ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
    //public async Task<IActionResult> Create(CreateIdeaCommand command)
    //{
    //    var result = await Mediator.Send(command);
    //    return Ok(result);
    //    //if (!result.status)
    //    //{
    //    //    return BadRequest(new Result
    //    //    {
    //    //        Succeeded = false,
    //    //        Errors = new string[] { result.message },
    //    //        Data = result.response
    //    //    });
    //    //}
    //    //return Ok(new Result
    //    //{
    //    //    Succeeded = true, 
    //    //    Errors = null,
    //    //    Data = result.response
    //    //});
    //}
    //[HttpPost]
    //[ProducesResponseType(typeof(Result), (int)HttpStatusCode.OK)]
    //[ProducesResponseType(typeof(Result), (int)HttpStatusCode.BadRequest)]
    //public async Task<IActionResult> ActionMethodSample(object request)
    //{
    //    try
    //    {
    //        if (request == null)
    //        {
    //            return BadRequest(new ApiResponse<string>
    //            {
    //                Code = SystemCodes.Failed,
    //                Data = string.Empty,
    //                Description = "Empty Request",
    //            });
    //        }

    //        //a call the service to the work

    //        return Ok(new ApiResponse<string>
    //        {
    //            Code = SystemCodes.Successful,
    //            Data = string.Empty,
    //            Description = "Empty Request",
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new ApiResponse<string>
    //        {
    //            Code = SystemCodes.Failed,
    //            Data = ex.Message,
    //            Description = ex.StackTrace,
    //        });
    //    }
    //}

    //// GET: api/<IdeaController>
    //[HttpGet]
    //public IEnumerable<string> Get()
    //{
    //    return new string[] { "value1", "value2" };
    //}

    //// GET api/<IdeaController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<IdeaController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<IdeaController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<IdeaController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
    #endregion
}