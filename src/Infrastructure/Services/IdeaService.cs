using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;
public class IdeaService : IIdeaService
{
    public Task<CommentResponse> CommentOnIdea(CommentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IdeaResponse> CreateIdea(CreateIdeaRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteComment(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteDirectMessage(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteIdea(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteUserCreatedList(string listId)
    {
        throw new NotImplementedException();
    }

    public Task<string> FollowUser(string username)
    {
        throw new NotImplementedException();
    }

    public Task<List<Idea>> GetAllIdeas()
    {
        throw new NotImplementedException();
    }

    public Task<MessagesResponse> GetDirectMessages()
    {
        throw new NotImplementedException();
    }

    public Task<ExploreModel> GetExplore()
    {
        throw new NotImplementedException();
    }

    public Task<List<Idea>> GetFeed()
    {
        throw new NotImplementedException();
    }

    public Task<List<Idea>> GetIdeasFromUser(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<AccountSettings> GetUserAccountSettings()
    {
        throw new NotImplementedException();
    }

    public Task<UserCreatedListResponse> GetUserCreatedLists(UserCreatedListRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<EngagementAnalytics> GetUserEngagementAnalytics()
    {
        throw new NotImplementedException();
    }

    public Task<NotificationResponse> GetUserNotifications()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileModel> GetUserProfile(string username)
    {
        throw new NotImplementedException();
    }

    public Task<string> LikeIdea(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<SearchModel> Search(string query)
    {
        throw new NotImplementedException();
    }

    public Task<string> SendDirectMessage(DirectMessageRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ShareIdeaResponse> ShareIdea(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> UnfollowUser(string username)
    {
        throw new NotImplementedException();
    }

    public Task<string> UnlikeIdea(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> UnshareIdea(string ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateUserAccountSettings(UpdateAccountSettingsRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateUserCreatedList(UpdateUserCreatedListRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateUserProfile(UpdateUserProfileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<UserTimelineModel> UserTimeline(string username)
    {
        throw new NotImplementedException();
    }
}
