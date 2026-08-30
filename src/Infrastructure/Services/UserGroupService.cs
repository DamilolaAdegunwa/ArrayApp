using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.GroupAggregate;
using ArrayApp.Infrastructure.Repositories.Interfaces;
using ArrayApp.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Services;

public class UserGroupService : IUserGroupService
{
    private readonly ILogger<UserGroupService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UserGroupService(ILogger<UserGroupService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserGroupDto> CreateUserGroupAsync(UserGroupCreateDto userGroupCreateDto)
    {
        _logger.LogInformation("Creating user group: {Name}", userGroupCreateDto.Name);
        var group = new UserGroup
        {
            Name = userGroupCreateDto.Name,
            Description = userGroupCreateDto.Description,
            Privacy = userGroupCreateDto.Privacy,
            Type = userGroupCreateDto.Type,
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };

        var saved = await _unitOfWork.UserGroupBaseRepository.AddAsync(group);
        return MapToDto(saved);
    }

    public async Task<UserGroupDto> GetUserGroupByIdAsync(int userGroupId)
    {
        var group = await _unitOfWork.UserGroupBaseRepository.GetByIdAsync(userGroupId);
        return group != null ? MapToDto(group) : new UserGroupDto();
    }

    public async Task<IEnumerable<UserGroupDto>> GetAllUserGroupsAsync()
    {
        var groups = await _unitOfWork.UserGroupBaseRepository.ListAsync();
        return groups.Select(MapToDto);
    }

    public async Task UpdateUserGroupAsync(int userGroupId, UserGroupUpdateDto userGroupUpdateDto)
    {
        var group = await _unitOfWork.UserGroupBaseRepository.GetByIdAsync(userGroupId);
        if (group != null)
        {
            if (!string.IsNullOrWhiteSpace(userGroupUpdateDto.Name)) group.Name = userGroupUpdateDto.Name;
            if (!string.IsNullOrWhiteSpace(userGroupUpdateDto.Description)) group.Description = userGroupUpdateDto.Description;
            if (!string.IsNullOrWhiteSpace(userGroupUpdateDto.Privacy)) group.Privacy = userGroupUpdateDto.Privacy;
            if (!string.IsNullOrWhiteSpace(userGroupUpdateDto.Type)) group.Type = userGroupUpdateDto.Type;
            group.ModifiedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.UserGroupBaseRepository.UpdateAsync(group);
        }
    }

    public async Task DeleteUserGroupAsync(int userGroupId)
    {
        var group = await _unitOfWork.UserGroupBaseRepository.GetByIdAsync(userGroupId);
        if (group != null)
        {
            await _unitOfWork.UserGroupBaseRepository.DeleteAsync(group);
        }
    }

    private static UserGroupDto MapToDto(UserGroup g) => new UserGroupDto
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        Privacy = g.Privacy,
        Type = g.Type,
        CreatedAt = g.CreatedAt
    };
}
