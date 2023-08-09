using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IIdeaService
{
    //`GET /idea/feed`: Get the main home feed with ideas from followed users. I need to figure out the algorithm for serving feeds
    Task<List<Idea>> GetFeed();

    //`GET /idea/timeline/{username}`: Get the user's timeline with their tweets.
    Task<UserTimelineModel> UserTimeline(string username);

    //`GET /idea/explore`: Get trending topics and suggested users.
    Task<ExploreModel> GetExplore();

    //`GET /idea/search`: Search for tweets, users, hashtags, etc.
    Task<SearchModel> Search(string query);
}
