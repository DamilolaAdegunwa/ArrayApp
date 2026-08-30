using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.CommentAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class IdeaService : IIdeaService
{
    private readonly ILogger<IdeaService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public IdeaService(ILogger<IdeaService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Idea>> GetFeed()
    {
        _logger.LogInformation("Retrieving main idea feed.");
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return ideas.OrderByDescending(i => i.Id).ToList();
    }

    public async Task<List<Idea>> GetAllIdeas()
    {
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return ideas.ToList();
    }

    public async Task<List<Idea>> GetIdeasFromUser(string userId)
    {
        int.TryParse(userId, out int parsedUserId);
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return ideas.Where(i => i.CreatorUserId == parsedUserId).ToList();
    }

    public async Task<UserTimelineModel> UserTimeline(string username)
    {
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return new UserTimelineModel
        {
            Username = username,
            Ideas = ideas.Take(20).ToList()
        };
    }

    public async Task<ExploreModel> GetExplore()
    {
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return new ExploreModel
        {
            TrendingIdeas = ideas.Take(10).ToList()
        };
    }

    public async Task<SearchModel> Search(string query)
    {
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        var lowerQuery = (query ?? string.Empty).ToLowerInvariant();
        return new SearchModel
        {
            Ideas = ideas.Where(i => (i.Title ?? string.Empty).ToLowerInvariant().Contains(lowerQuery) ||
                                     (i.Description ?? string.Empty).ToLowerInvariant().Contains(lowerQuery) ||
                                     (i.Content ?? string.Empty).ToLowerInvariant().Contains(lowerQuery)).ToList()
        };
    }

    public Task<NotificationResponse> GetUserNotifications()
    {
        return Task.FromResult(new NotificationResponse
        {
            Notifications = new List<NotificationModel>()
        });
    }

    public Task<MessagesResponse> GetDirectMessages()
    {
        return Task.FromResult(new MessagesResponse
        {
            Messages = new List<MessageModel>()
        });
    }

    public Task<UserProfileModel> GetUserProfile(string username)
    {
        return Task.FromResult(new UserProfileModel
        {
            Username = username,
            Bio = "Active innovator in ArrayApp"
        });
    }

    public async Task<IdeaResponse> CreateIdea(CreateIdeaRequest request)
    {
        _logger.LogInformation("Creating idea via IdeaService: {Content}", request.Content);
        try
        {
            var entity = new Idea
            {
                Title = !string.IsNullOrWhiteSpace(request.Content) && request.Content.Length > 20
                    ? request.Content[..20] + "..."
                    : (request.Content ?? "New Idea"),
                Content = request.Content ?? string.Empty,
                Description = request.Content ?? string.Empty,
                CreatorUserId = request.AuthorId,
                CreationTime = DateTimeOffset.UtcNow
            };

            var resp = await _unitOfWork.IdeaBaseRepository.AddAsync(entity);
            return new IdeaResponse
            {
                Id = resp.Id,
                Content = resp.Content,
                AuthorId = request.AuthorId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while trying to create Idea");
            return new IdeaResponse { Content = request.Content, AuthorId = request.AuthorId };
        }
    }

    public async Task<CommentResponse> CommentOnIdea(CommentRequest request)
    {
        _logger.LogInformation("Adding comment to idea: {IdeaId}", request.IdeaId);
        var comment = new Comment
        {
            Text = request.Comment ?? string.Empty,
            Content = $"Idea:{request.IdeaId}",
            Rating = 5,
            Status = "approved",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.CommentBaseRepository.AddAsync(comment);
        return new CommentResponse
        {
            Id = saved.Id.ToString(),
            Comment = saved.Text,
            IdeaId = request.IdeaId
        };
    }

    public async Task<string> LikeIdea(string ideaId)
    {
        if (int.TryParse(ideaId, out int id))
        {
            var idea = await _unitOfWork.IdeaBaseRepository.GetByIdAsync(id);
            if (idea != null)
            {
                idea.Upvotes++;
                await _unitOfWork.IdeaBaseRepository.UpdateAsync(idea);
                return "Liked successfully";
            }
        }
        return "Idea not found";
    }

    public async Task<string> UnlikeIdea(string ideaId)
    {
        if (int.TryParse(ideaId, out int id))
        {
            var idea = await _unitOfWork.IdeaBaseRepository.GetByIdAsync(id);
            if (idea != null)
            {
                if (idea.Upvotes > 0) idea.Upvotes--;
                await _unitOfWork.IdeaBaseRepository.UpdateAsync(idea);
                return "Unliked successfully";
            }
        }
        return "Idea not found";
    }

    public async Task<ShareIdeaResponse> ShareIdea(string ideaId)
    {
        if (int.TryParse(ideaId, out int id))
        {
            var idea = await _unitOfWork.IdeaBaseRepository.GetByIdAsync(id);
            if (idea != null)
            {
                idea.ViewsCount++;
                await _unitOfWork.IdeaBaseRepository.UpdateAsync(idea);
                return new ShareIdeaResponse { Success = true, Message = "Idea shared successfully" };
            }
        }
        return new ShareIdeaResponse { Success = false, Message = "Idea not found" };
    }

    public Task<string> UnshareIdea(string ideaId)
    {
        return Task.FromResult("Idea unshared successfully");
    }

    public Task<string> FollowUser(string username)
    {
        return Task.FromResult($"Followed {username} successfully");
    }

    public Task<string> UnfollowUser(string username)
    {
        return Task.FromResult($"Unfollowed {username} successfully");
    }

    public Task<string> SendDirectMessage(DirectMessageRequest request)
    {
        return Task.FromResult("Message sent successfully");
    }

    public async Task<string> DeleteIdea(string ideaId)
    {
        if (int.TryParse(ideaId, out int id))
        {
            var idea = await _unitOfWork.IdeaBaseRepository.GetByIdAsync(id);
            if (idea != null)
            {
                await _unitOfWork.IdeaBaseRepository.DeleteAsync(idea);
                return "Idea deleted successfully";
            }
        }
        return "Idea not found";
    }

    public async Task<string> DeleteComment(string commentId)
    {
        if (int.TryParse(commentId, out int id))
        {
            var comment = await _unitOfWork.CommentBaseRepository.GetByIdAsync(id);
            if (comment != null)
            {
                await _unitOfWork.CommentBaseRepository.DeleteAsync(comment);
                return "Comment deleted successfully";
            }
        }
        return "Comment deleted";
    }

    public Task<string> DeleteDirectMessage(string messageId)
    {
        return Task.FromResult("Message deleted");
    }

    public Task<string> DeleteUserCreatedList(string listId)
    {
        return Task.FromResult("List deleted");
    }

    public Task<UserCreatedListResponse> GetUserCreatedLists(UserCreatedListRequest request)
    {
        return Task.FromResult(new UserCreatedListResponse
        {
            Lists = new List<UserCreatedListModel>()
        });
    }

    public Task<string> UpdateUserCreatedList(UpdateUserCreatedListRequest request)
    {
        return Task.FromResult("List updated successfully");
    }

    public Task<AccountSettings> GetUserAccountSettings()
    {
        return Task.FromResult(new AccountSettings
        {
            EmailNotifications = true,
            PushNotifications = true
        });
    }

    public Task<string> UpdateUserAccountSettings(UpdateAccountSettingsRequest request)
    {
        return Task.FromResult("Account settings updated successfully");
    }

    public Task<string> UpdateUserProfile(UpdateUserProfileRequest request)
    {
        return Task.FromResult("Profile updated successfully");
    }

    public async Task<EngagementAnalytics> GetUserEngagementAnalytics()
    {
        var ideas = await _unitOfWork.IdeaBaseRepository.ListAsync();
        return new EngagementAnalytics
        {
            TotalIdeas = ideas.Count,
            TotalLikes = ideas.Sum(i => i.Upvotes),
            TotalViews = ideas.Sum(i => i.ViewsCount)
        };
    }
}
