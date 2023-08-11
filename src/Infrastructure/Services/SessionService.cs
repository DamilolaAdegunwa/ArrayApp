using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;

namespace ArrayApp.Infrastructure.Services;

public class SessionService : ISessionService
{
    public Task<SessionDto> CreateSessionAsync(SessionCreateDto sessionCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSessionAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDto>> GetAllSessionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDto>> GetPastSessionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SessionDto> GetSessionByIdAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDto>> GetSessionsByIdeaAsync(int ideaId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDto>> GetSessionsByUserAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalSessionCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDto>> GetUpcomingSessionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SessionDto> UpdateSessionAsync(int sessionId, SessionUpdateDto sessionUpdateDto)
    {
        throw new NotImplementedException();
    }
}
