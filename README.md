# 💡 ArrayApp (IdeaApp) — Developer & Platform Reference

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![React 18](https://img.shields.io/badge/React-18.0-61DAFB?logo=react&logoColor=black)](https://react.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.4-38B2AC?logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-blue?logo=signalr&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Build Status](https://img.shields.io/badge/Build-Passing-22c55e?logo=github-actions&logoColor=white)](#)

> **ArrayApp (IdeaApp)** is an enterprise idea cultivation, maturation, collaboration, and work orchestration platform. This reference documentation details the feature roadmap, EF Core migrations, database configurations, controller endpoints, JSON queries, and JWT authentication schemas.

---

## 📑 Table of Contents
1. [Feature Roadmap & Backlog Checklist](#-feature-roadmap--backlog-checklist)
2. [Database & EF Core Migrations](#-database--ef-core-migrations)
3. [Database Servers & Connection Targets](#-database-servers--connection-targets)
4. [Controller Architecture & Blueprint](#-controller-architecture--blueprint)
5. [HomeController Endpoints (Rough Sketch)](#-homecontroller-endpoints-rough-sketch)
6. [LINQ to SQL & JSON Query Patterns](#-linq-to-sql--json-query-patterns)
7. [Authentication, JWT & Security Reference](#-authentication-jwt--security-reference)

---

## 🚀 Feature Roadmap & Backlog Checklist

Below is the comprehensive catalog of features, tracking tools, and social collaboration capabilities:

### Core User & Social Features
- [x] Implement a login system for users. ✔️
- [ ] Add the ability for users to create and save ideas.
- [ ] Design a user profile page.
- [ ] Implement a search function to allow users to find ideas.
- [ ] Add the ability for users to comment on and rate ideas.
- [ ] Implement a notification system to alert users when their ideas have been commented on or rated.
- [ ] Add the ability for users to categorize their ideas.
- [ ] Implement a system for users to collaborate on ideas.
- [ ] Add the ability for users to share their ideas on social media.
- [ ] Implement a system to track user activity and engagement on the app.
- [ ] Implement a recommendation system to suggest ideas to users based on their interests.
- [ ] Add the ability for users to upload images and videos to their ideas.
- [ ] Implement a system to monitor and moderate user-generated content.
- [ ] Add the ability for users to create and join groups based on their interests.
- [ ] Implement a system to track user achievements and rewards.
- [ ] Add the ability for users to set goals and track their progress.
- [ ] Implement a system for users to challenge each other to complete ideas.
- [ ] Add the ability for users to search for other users and connect with them.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to customize their ideaapp experience by choosing different themes and layouts.
- [ ] Add the ability for users to create and manage to-do lists.
- [ ] Implement a system for users to set reminders and deadlines for their ideas.
- [ ] Add the ability for users to create and share polls and surveys.
- [ ] Implement a system to track user feedback and suggestions.
- [ ] Add the ability for users to share and collaborate on mind maps and diagrams.
- [ ] Implement a system for users to track their progress on ideas and set benchmarks.
- [ ] Add the ability for users to create and share calendars and schedules.
- [ ] Implement a system for users to track their ideas and goals over time.
- [ ] Add the ability for users to set privacy settings for their ideas.
- [ ] Implement a system for users to share and collaborate on documents and files.
- [ ] Add the ability for users to create and share audio and video recordings.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share quizzes and games.
- [ ] Implement a system for users to receive notifications and alerts for important events.
- [ ] Add the ability for users to create and share presentations and slideshows.
- [ ] Implement a system for users to share and collaborate on projects and presentations.
- [ ] Add the ability for users to create and share notes and notes.

<details>
<summary><strong>🔍 Click to expand full list of extended media & domain tracking capabilities (100+ items)</strong></summary>

### Media, Visuals & Authoring Tools
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share polls and surveys.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share images and graphics.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share infographics and data visualizations.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share web pages and blogs.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share podcasts and audio recordings.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share videos and animations.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share ebooks and PDFs.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share mind maps and diagrams.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share interactive games and quizzes.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share forms and surveys.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share podcasts and audio recordings.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share reports and presentations.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share worksheets and spreadsheets.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share calendars and schedules.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share timelines and graphics.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share diagrams and flowcharts.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share mind maps and concept maps.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share graphs and charts.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share infographics and data visualizations.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share videos and animations.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share web pages and blogs.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share polls and surveys.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share quizzes and games.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share flashcards and study guides.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share puzzles and brainteasers.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share mazes and crosswords.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share riddles and jokes.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share puzzles and games.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share stories and poems.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share drawings and paintings.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share collages and scrapbooks.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share photo albums and galleries.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.

### Lifestyle, Planning & Productivity Tracking
- [ ] Add the ability for users to create and share recipes and meal plans.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share DIY projects and tutorials.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share crafts and projects.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share gardening and plant care guides.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share workout plans and exercises.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share travel guides and itineraries.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share reviews and ratings.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share gift ideas and lists.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share wish lists and recommendations.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share budget and finance tracking tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share event planning and organization tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share task and project management tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share shopping lists and grocery lists.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share checklists and to-do lists.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share note-taking and journaling tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share calendar and scheduling tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share recipe and meal planning tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share homework and study tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share fitness and wellness tracking tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share music and audio playback tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share photo and image editing tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share video and movie playback tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share book and ebook reading tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share weather and forecast tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share social media and communication tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share news and information tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share language and translation tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share gardening and plant care tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share DIY and home improvement tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share fashion and styling tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share health and wellness tracking tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share pet care and training tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share travel planning and organization tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.
- [ ] Add the ability for users to create and share hobby and interest tracking tools.
- [ ] Implement a system for users to track their ideaapp usage and progress over time.
- [ ] Add the ability for users to create and share language learning and translation tools.
- [ ] Implement a system for users to track their ideaapp usage and activity.

</details>

---

## 🛠️ Database & EF Core Migrations

Commands for managing database schema migrations using Entity Framework Core:

### 1. Create a Migration
```powershell
# Package Manager Console
Add-Migration MyDemoMigration

# .NET CLI
dotnet ef migrations add MyDemoMigration
```

### 2. Apply Migrations to Database
```powershell
# Package Manager Console
Update-Database

# .NET CLI
dotnet ef database update
```

### 3. Remove the Last Migration
```powershell
# Package Manager Console
Remove-Migration

# .NET CLI
dotnet ef migrations remove
```

### 4. Revert to a Specific Migration
```powershell
# Package Manager Console
Update-Database MyInitialMigration

# .NET CLI
dotnet ef database update MyInitialMigration
```

---

## 🖥️ Database Servers & Connection Targets

| Target Server | Description | Environment |
| :--- | :--- | :--- |
| `(localdb)\mssqllocaldb` | LocalDB Default Instance | Development (Windows) |
| `.` | Localhost / Default SQL Server Instance | Local Dev |
| `LAPTOP-87BT59K6` | Dedicated Local Development Workstation | Machine Hostname |

---

## 🏛️ Controller Architecture & Blueprint

Exhaustive controller blueprint for building an idea cultivation platform & social ecosystem:

| # | Controller | Primary Responsibility |
| :---: | :--- | :--- |
| 1 | `HomeController` | Responsible for handling the main home feed and user timelines. |
| 2 | `UserController` | Handles user profiles, following/unfollowing users, and user-related actions. |
| 3 | `TweetController` | Manages creating, deleting, and interacting with ideas. |
| 4 | `CommentController` | Handles commenting on ideas and interacting with comments. |
| 5 | `LikeController` | Manages liking and unliking ideas. |
| 6 | `ReideaController` | Handles reideaing and unreideaing ideas. |
| 7 | `NotificationController` | Manages user notifications, such as mentions and likes. |
| 8 | `HashtagController` | Handles searching for and displaying ideas with specific hashtags. |
| 9 | `ExploreController` | Displays trending topics, suggested users, and popular ideas. |
| 10 | `MessageController` | Manages direct messaging and conversations between users. |
| 11 | `SearchController` | Handles searching for users, ideas, hashtags, etc. |
| 12 | `SettingsController` | Manages user account settings, privacy, and preferences. |
| 13 | `ReportController` | Handles reporting ideas, users, or other content. |
| 14 | `BlockController` | Manages blocking and unblocking users. |
| 15 | `FollowRequestController` | Handles user follow requests and approvals. |
| 16 | `ListController` | Manages user-created lists of other users. |
| 17 | `AnalyticsController` | Provides analytics and insights for user engagement. |
| 18 | `APIController` | Offers endpoints for interacting with the application programmatically. |
| 19 | `AuthController` | Manages user authentication, registration, and password reset. |
| 20 | `AdminPanelController` | Controls administrative tasks for managing users and content. |

---

## 📡 HomeController Endpoints (Rough Sketch)

```http
### Feeds & Timelines
GET    /idea/feed                       # Get the main idea feed with ideas from followed users.
GET    /idea/timeline/{username}        # Get the user's timeline with their ideas.
GET    /idea/explore                    # Get trending topics and suggested users.

### Search & Discovery
GET    /idea/search                     # Search for ideas, users, hashtags, etc.

### Notifications & Communication
GET    /idea/notifications              # Get user notifications (mentions, likes, etc.).
GET    /idea/messages                   # Get direct messages and conversations.
POST   /idea/message                    # Send a direct message.
DELETE /idea/message/{messageId}        # Delete a direct message.

### Profiles & Relationships
GET    /idea/profile/{username}         # Get user profile information.
PUT    /idea/profile/update             # Update user profile information.
POST   /idea/follow/{username}          # Follow a user.
POST   /idea/unfollow/{username}        # Unfollow a user.

### Idea Management & Engagement
POST   /idea                            # Create a new idea.
DELETE /idea/idea/{ideaId}              # Delete a idea.
POST   /idea/comment                    # Comment on a idea.
DELETE /idea/comment/{commentId}        # Delete a comment.
POST   /idea/like/{ideaId}              # Like a idea.
POST   /idea/unlike/{ideaId}            # Unlike a idea.
POST   /idea/reidea/{ideaId}            # Reidea a idea.
POST   /idea/unreidea/{ideaId}          # Unreidea a idea.

### User Settings & Preferences
GET    /idea/settings                   # Get user account settings.
PUT    /idea/settings/update            # Update user account settings.

### User Lists
GET    /idea/lists                      # Get user-created lists.
GET    /idea/lists/{listId}             # Get ideas from a specific user list.
POST   /idea/lists/create               # Create a new user list.
PUT    /idea/lists/{listId}/update      # Update a user list.
DELETE /idea/lists/{listId}/delete      # Delete a user list.

### Analytics & Administration
GET    /idea/analytics                  # Get user engagement analytics.
GET    /idea/admin                      # Access administrative panel (if applicable).
```

---

## 🔍 LINQ to SQL & JSON Query Patterns

Examples of mapping C# LINQ queries to raw T-SQL queries with SQL Server JSON support (`JSON_VALUE`, `JSON_QUERY`, `JSON_MODIFY`):

### 1. Filter Authors by JSON City Property
**C# LINQ:**
```csharp
var authorsInChigley = await context.Authors
    .Where(author => author.Contact.Address.City == "Chigley")
    .ToListAsync();
```
**Generated SQL:**
```sql
SELECT
    [a].[Id],
    [a].[Name],
    JSON_QUERY([a].[Contact], '$') AS Contact
FROM
    [Authors] AS [a]
WHERE
    CAST(JSON_VALUE([a].[Contact], '$.Address.City') AS nvarchar(max)) = N'Chigley';
```

---

### 2. Category Lookup by Parameter
**T-SQL:**
```sql
DECLARE @p0 NVARCHAR(50);
SET @p0 = 'Editor''s Picks';
SELECT * FROM [ArrayAppDb].[dbo].[Categories] WHERE Name = @p0;
```

---

### 3. Update Contact Address JSON Column
**T-SQL:**
```sql
UPDATE [Authors]
SET [Contact] = JSON_MODIFY([Contact], 'strict $.Address', JSON_QUERY(@p0))
OUTPUT 1
WHERE [Id] = @p1;
```

---

### 4. Extract Postcode from JSON Contact
**C# LINQ:**
```csharp
var postcodesInChigley = await context.Authors
    .Where(author => author.Contact.Address.City == "Chigley")
    .Select(author => author.Contact.Address.Postcode)
    .ToListAsync();
```
**Generated SQL:**
```sql
SELECT
    CAST(JSON_VALUE([a].[Contact], '$.Address.Postcode') AS nvarchar(max))
FROM
    [Authors] AS [a]
WHERE
    CAST(JSON_VALUE([a].[Contact], '$.Address.City') AS nvarchar(max)) = N'Chigley';
```

---

### 5. Filter and Concatenate Formatted Address
**C# LINQ:**
```csharp
var orderedAddresses = await context.Authors
    .Where(author => (author.Contact.Address.City == "Chigley" && author.Contact.Phone != null)
                  || author.Name.StartsWith("D"))
    .OrderBy(author => author.Contact.Phone)
    .Select(author => author.Name + " (" +
                     author.Contact.Address.Street + ", " +
                     author.Contact.Address.City + " " +
                     author.Contact.Address.Postcode + ")")
    .ToListAsync();
```
**Generated SQL:**
```sql
SELECT ((((((([a].[Name] + N' (' +
             CAST(JSON_VALUE([a].[Contact], '$.Address.Street') AS nvarchar(max))) + N', ' +
             CAST(JSON_VALUE([a].[Contact], '$.Address.City') AS nvarchar(max))) + N' ') +
         CAST(JSON_VALUE([a].[Contact], '$.Address.Postcode') AS nvarchar(max))) + N')'
FROM [Authors] AS [a]
WHERE (CAST(JSON_VALUE([a].[Contact], '$.Address.City') AS nvarchar(max)) = N'Chigley'
       AND CAST(JSON_VALUE([a].[Contact], '$.Phone') AS nvarchar(max)) IS NOT NULL)
       OR ([a].[Name] LIKE N'D%')
ORDER BY CAST(JSON_VALUE([a].[Contact], '$.Phone') AS nvarchar(max));
```

---

### 6. Projecting JSON Post Metadata & Views
**C# LINQ:**
```csharp
var postsWithViews = await context.Posts
    .Where(post => post.Metadata!.Views > 3000)
    .AsNoTracking()
    .Select(post => new
    {
        post.Author!.Name,
        Views = post.Metadata!.Views,
        Searches = post.Metadata.TopSearches,
        Commits = post.Metadata.Updates
    })
    .ToListAsync();
```
**Generated SQL:**
```sql
SELECT [a].[Name], 
       CAST(JSON_VALUE([p].[Metadata], '$.Views') AS int),
       JSON_QUERY([p].[Metadata], '$.TopSearches'),
       [p].[Id], 
       JSON_QUERY([p].[Metadata], '$.Updates')
FROM [Posts] AS [p]
LEFT JOIN [Authors] AS [a] ON [p].[AuthorId] = [a].[Id]
WHERE CAST(JSON_VALUE([p].[Metadata], '$.Views') AS int) > 3000;
```

---

## 🔐 Authentication, JWT & Security Reference

### 1. Login Credentials & Payloads

#### Sample User Credential (Type 0)
```json
{
  "userName": "damee1993@gmail.com",
  "password": "Damilola#123",
  "userType": 0
}
```

#### Alternate Test User Credential
```json
{
  "email": "damee1993@gmail.com",
  "password": "Password#123"
}
```

---

### 2. Sample Login API Response

```json
{
  "code": "200",
  "shortDescription": "SUCCESS",
  "object": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImQxOTk1YmJiLWI0NmMtNGI5YS1iZmQ4LWVkZDQzYjAxNWRiYiIsIm5hbWUiOiJkYW1lZTE5OTNAZ21haWwuY29tIiwiZW1haWwiOiJkYW1lZTE5OTNAZ21haWwuY29tIiwibmJmIjoxNjk3NDQ2OTQ3LCJleHAiOjE2OTc2MjY5NDgsImlzcyI6IkFycmF5QXBwLldlYlVJLklzc3VlciIsImF1ZCI6IkFycmF5QXBwLldlYlVJLkF1ZGllbmNlIn0.JogQrMoXDTNTH8LhXMaUBgMhA6HgwCua-dstIQLRYL4",
    "refreshToken": "xgXLPGLbfjBwY8LaJR4M72/YhTj/zELklvBUWikwAsg=",
    "expires": "2023-10-18T12:02:28.0021367+01:00"
  },
  "validationErrors": {}
}
```

---

### 3. Decoded JWT Tokens

#### Header & Payload Schema
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
.
{
  "sub": "your_username",
  "jti": "53eac3e5-ec8f-43eb-bd20-367a6075980a",
  "exp": 1697456969,
  "iss": "https://www.example.com",
  "aud": "WebAppVerify_Audience"
}
```

#### Role Claims Token Example
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkYW1pbG9sYV9hZGVndW53YSIsImp0aSI6IjE1MWJlNjEzLTEzODgtNDI0NS05YmY3LWQzMWY0NWU1ZmM4YSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJhZG1pbiIsIm1hbmFnZXIiXSwiZXhwIjoxNjk3NTc1NzU3LCJpc3MiOiJXZWJBcHBWZXJpZnlfSXNzdWVyIiwiYXVkIjoiV2ViQXBwVmVyaWZ5X0F1ZGllbmNlIn0.5c_10rD7oHEsw2gSuVjezAvMrkMug1-pX8B8-czPURo"
}
```

---

### 4. TokenValidationParameters Reference

```csharp
#region others
//--
//ActorValidationParameters = default,
//AlgorithmValidator = default,
//AudienceValidator = default,
//AuthenticationType = default,
//ClockSkew = default,
//ConfigurationManager = default,
//CryptoProviderFactory = default,
//IgnoreTrailingSlashWhenValidatingAudience = default,
//IssuerSigningKeyResolver = default,
//IssuerSigningKeyResolverUsingConfiguration = default,
//IssuerSigningKeys = default,
//IssuerSigningKeyValidator = default,
//IssuerSigningKeyValidatorUsingConfiguration = default,
//ValidAlgorithms = default,
//IssuerValidator = default,
//IssuerValidatorUsingConfiguration = default,
//LifetimeValidator = default,
//NameClaimType = default,
//NameClaimTypeRetriever = default,
//PropertyBag = default,
//RequireAudience = default,
//RequireExpirationTime = default,
//RequireSignedTokens = default,
//RoleClaimType = default,
//RoleClaimTypeRetriever = default,
//SaveSigninToken = default,
//SignatureValidator = default,
//SignatureValidatorUsingConfiguration = default,
//TokenDecryptionKey = default,
//TryAllIssuerSigningKeys = default,
//TokenDecryptionKeyResolver = default,
//TokenDecryptionKeys = default,
//TokenReader = default,
//TokenReplayCache = default,
//ValidateActor = default,
//ValidAudiences = default,
//TokenReplayValidator = default,
//TypeValidator = default,
//ValidateLifetime = default,
//ValidateTokenReplay = default,
//ValidIssuers = default,
//ValidTypes = default,
#endregion
```