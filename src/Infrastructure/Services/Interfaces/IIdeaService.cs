using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IIdeaService
{
    //`GET /idea/feed`: Get the main home feed with ideas from followed users. I need to figure out the algorithm for serving feeds
    Task<List<Idea>> GetFeed();

    //`GET /idea/timeline/{username}`: Get the user's timeline with their ideas.
    Task<UserTimelineModel> UserTimeline(string username);

    //`GET /idea/explore`: Get trending topics and suggested users.
    Task<ExploreModel> GetExplore();

    //`GET /idea/search`: Search for ideas, users, hashtags, etc.
    Task<SearchModel> Search(string query);

    //`GET /idea/notifications`: Get user notifications (mentions, likes, etc.).
    Task<NotificationResponse> GetUserNotifications();

    //`GET /idea/messages`: Get direct messages and conversations.
    Task<MessagesResponse> GetDirectMessages();

    //`GET /idea/profile/{username}`: Get user profile information.
    Task<UserProfileModel> GetUserProfile(string username);

    //`POST /idea`: Create a new idea.
    Task<IdeaResponse> CreateIdea(CreateIdeaRequest request);

    //`POST /idea/comment`: Comment on a idea.
    Task<CommentResponse> CommentOnIdea(CommentRequest request);

    //`POST /idea/like/{ideaId}`: Like a idea.
    Task<string> LikeIdea(string ideaId);

    //`POST /idea/unlike/{ideaId}`: Unlike a idea.
    Task<string> UnlikeIdea(string ideaId);

    //`POST /idea/share-idea/{ideaId}`: share idea.
    Task<ShareIdeaResponse> ShareIdea(string ideaId);

    //`POST /idea/unshare-idea/{ideaId}`: unshare idea.
    Task<string> UnshareIdea(string ideaId);

    //`POST /idea/follow/{username}`: Follow a user.
    Task<string> FollowUser(string username);

    Task<string> UnfollowUser(string username);

    //`POST /idea/message`: Send a direct message.
    Task<string> SendDirectMessage(DirectMessageRequest request);

    //`PUT /idea/profile/update`: Update user profile information.
    Task<string> UpdateUserProfile(UpdateUserProfileRequest request);

    //`DELETE /idea/idea/{ideaId}`: Delete a idea.
    Task<string> DeleteIdea(string ideaId);

    //`DELETE /idea/comment/{commentId}`: Delete a comment.
    Task<string> DeleteComment(string ideaId);

    //`DELETE /idea/message/{messageId}`: Delete a message.
    Task<string> DeleteDirectMessage(string ideaId);

    //`GET /idea/settings`: Get user account settings.
    Task<AccountSettings> GetUserAccountSettings();

    //`PUT /idea/settings/update`: Update user account settings.
    Task<string> UpdateUserAccountSettings(UpdateAccountSettingsRequest request);


    Task<List<Idea>> GetAllIdeas();
    Task<List<Idea>> GetIdeasFromUser(string userId);

    Task<UserCreatedListResponse> GetUserCreatedLists(UserCreatedListRequest request);

    Task<string> UpdateUserCreatedList(UpdateUserCreatedListRequest request);

    Task<string> DeleteUserCreatedList(string listId);

    Task<EngagementAnalytics> GetUserEngagementAnalytics();
}
