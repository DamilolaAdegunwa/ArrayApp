using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface ISessionService
{
    Task<SessionDto> CreateSessionAsync(SessionCreateDto sessionCreateDto);
    Task<SessionDto> GetSessionByIdAsync(int sessionId);
    Task<IEnumerable<SessionDto>> GetAllSessionsAsync();
    Task<IEnumerable<SessionDto>> GetSessionsByUserAsync(int userId);
    Task<IEnumerable<SessionDto>> GetSessionsByIdeaAsync(int ideaId);
    Task<IEnumerable<SessionDto>> GetUpcomingSessionsAsync();
    Task<IEnumerable<SessionDto>> GetPastSessionsAsync();
    Task<int> GetTotalSessionCountAsync();
    Task<SessionDto> UpdateSessionAsync(int sessionId, SessionUpdateDto sessionUpdateDto);
    Task DeleteSessionAsync(int sessionId);
}
