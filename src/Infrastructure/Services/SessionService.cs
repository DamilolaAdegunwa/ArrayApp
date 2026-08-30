using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Enums;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public SessionService(ILogger<SessionService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<SessionDto> CreateSessionAsync(SessionCreateDto sessionCreateDto)
    {
        _logger.LogInformation("Creating session: {Name}", sessionCreateDto.Name);
        var session = new Session
        {
            Name = sessionCreateDto.Name,
            Description = sessionCreateDto.Description,
            Type = sessionCreateDto.Type,
            PrimaryIdeaId = sessionCreateDto.PrimaryIdeaId,
            ScheduledStartTime = sessionCreateDto.ScheduledStartTime,
            Duration = TimeSpan.FromMinutes(sessionCreateDto.DurationMinutes > 0 ? sessionCreateDto.DurationMinutes : 60),
            SessionStatus = SessionStatus.Scheduled,
            Status = "Scheduled",
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.SessionBaseRepository.AddAsync(session);
        return MapToDto(saved);
    }

    public async Task<SessionDto> GetSessionByIdAsync(int sessionId)
    {
        var session = await _unitOfWork.SessionBaseRepository.GetByIdAsync(sessionId);
        return session != null ? MapToDto(session) : new SessionDto();
    }

    public async Task<IEnumerable<SessionDto>> GetAllSessionsAsync()
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Select(MapToDto);
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsByUserAsync(int userId)
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Select(MapToDto);
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsByIdeaAsync(int ideaId)
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Where(s => s.PrimaryIdeaId == ideaId).Select(MapToDto);
    }

    public async Task<IEnumerable<SessionDto>> GetUpcomingSessionsAsync()
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Where(s => s.ScheduledStartTime >= DateTimeOffset.UtcNow).Select(MapToDto);
    }

    public async Task<IEnumerable<SessionDto>> GetPastSessionsAsync()
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Where(s => s.ScheduledStartTime < DateTimeOffset.UtcNow).Select(MapToDto);
    }

    public async Task<int> GetTotalSessionCountAsync()
    {
        var sessions = await _unitOfWork.SessionBaseRepository.ListAsync();
        return sessions.Count;
    }

    public async Task<SessionDto> UpdateSessionAsync(int sessionId, SessionUpdateDto sessionUpdateDto)
    {
        var session = await _unitOfWork.SessionBaseRepository.GetByIdAsync(sessionId);
        if (session != null)
        {
            if (!string.IsNullOrWhiteSpace(sessionUpdateDto.Name)) session.Name = sessionUpdateDto.Name;
            if (!string.IsNullOrWhiteSpace(sessionUpdateDto.Description)) session.Description = sessionUpdateDto.Description;
            if (!string.IsNullOrWhiteSpace(sessionUpdateDto.Status)) session.Status = sessionUpdateDto.Status;
            if (sessionUpdateDto.ScheduledStartTime.HasValue) session.ScheduledStartTime = sessionUpdateDto.ScheduledStartTime.Value;
            session.ModifiedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SessionBaseRepository.UpdateAsync(session);
            return MapToDto(session);
        }

        return new SessionDto { Id = sessionId };
    }

    public async Task DeleteSessionAsync(int sessionId)
    {
        var session = await _unitOfWork.SessionBaseRepository.GetByIdAsync(sessionId);
        if (session != null)
        {
            await _unitOfWork.SessionBaseRepository.DeleteAsync(session);
        }
    }

    private static SessionDto MapToDto(Session s) => new SessionDto
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        Type = s.Type,
        Status = s.Status,
        PrimaryIdeaId = s.PrimaryIdeaId,
        ScheduledStartTime = s.ScheduledStartTime,
        DurationMinutes = s.Duration.TotalMinutes,
        CreatedAt = s.CreatedAt
    };
}
