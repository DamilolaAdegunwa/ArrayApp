using Microsoft.AspNetCore.Mvc;
using System.Net;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using Microsoft.AspNetCore.Authorization;
using ArrayApp.Infrastructure.Services.Interfaces;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Duende.IdentityServer.Models;

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
            // Call the service to search for tweets, users, hashtags, etc.
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

    [HttpPost("tweet")]
    [ProducesResponseType(typeof(ApiResponse<TweetResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateTweet([FromBody] CreateTweetRequest request)
    {
        try
        {
            // Call the service to create a new tweet
            var result = await _ideaService.CreateTweet(request);

            return Ok(new ApiResponse<TweetResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Tweet created successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating a tweet");
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
    public async Task<IActionResult> CommentOnTweet([FromBody] CommentRequest request)
    {
        try
        {
            // Call the service to add a comment to a tweet
            var result = await _ideaService.CommentOnTweet(request);

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

    [HttpPost("like/{tweetId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> LikeTweet(string tweetId)
    {
        try
        {
            // Call the service to like a tweet
            await _ideaService.LikeTweet(tweetId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Tweet liked successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while liking a tweet");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("unlike/{tweetId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UnlikeTweet(string tweetId)
    {
        try
        {
            // Call the service to unlike a tweet
            await _ideaService.UnlikeTweet(tweetId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Tweet unliked successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unliking a tweet");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }

    [HttpPost("retweet/{tweetId}")]
    [ProducesResponseType(typeof(ApiResponse<RetweetResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Retweet(string tweetId)
    {
        try
        {
            // Call the service to retweet a tweet
            var result = await _ideaService.Retweet(tweetId);

            return Ok(new ApiResponse<RetweetResponse>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Tweet retweeted successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retweeting a tweet");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("unretweet/{tweetId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Unretweet(string tweetId)
    {
        try
        {
            // Call the service to unretweet a tweet
            await _ideaService.Unretweet(tweetId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Tweet unretweeted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unretweeting a tweet");
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

    [HttpDelete("tweet/{tweetId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteTweet(string tweetId)
    {
        try
        {
            // Call the service to delete a tweet
            await _ideaService.DeleteTweet(tweetId);

            return Ok(new ApiResponse<string>
            {
                Code = SystemCodes.Successful,
                Data = "Tweet deleted successfully",
                Description = string.Empty,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a tweet");
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
    [HttpGet("lists")]
    [ProducesResponseType(typeof(ApiResponse<List<UserList>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserCreatedLists()
    {
        try
        {
            // Call the service to get user-created lists
            var result = await _ideaService.GetUserCreatedLists();

            return Ok(new ApiResponse<List<UserList>>
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

    [HttpGet("lists/{listId}")]
    [ProducesResponseType(typeof(ApiResponse<List<Tweet>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTweetsFromUserList(string listId)
    {
        try
        {
            // Call the service to get tweets from a specific user list
            var result = await _ideaService.GetTweetsFromUserList(listId);

            return Ok(new ApiResponse<List<Tweet>>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "Tweets from user list retrieved successfully",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving tweets from user list");
            return BadRequest(new ApiResponse<string>
            {
                Code = SystemCodes.Failed,
                Data = ex.Message,
                Description = ex.StackTrace,
            });
        }
    }
    [HttpPost("lists/create")]
    [ProducesResponseType(typeof(ApiResponse<UserList>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateUserList([FromBody] CreateUserListRequest request)
    {
        try
        {
            // Call the service to create a new user list
            var result = await _ideaService.CreateUserList(request);

            return Ok(new ApiResponse<UserList>
            {
                Code = SystemCodes.Successful,
                Data = result,
                Description = "User list created successfully",
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

    [HttpPut("lists/{listId}/update")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserList(string listId, [FromBody] UpdateUserListRequest request)
    {
        try
        {
            // Call the service to update a user list
            await _ideaService.UpdateUserList(listId, request);

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
            await _ideaService.DeleteUserList(listId);

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