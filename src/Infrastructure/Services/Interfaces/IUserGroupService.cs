using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services.Interfaces;
public interface IUserGroupService
{
    Task<UserGroupDto> CreateUserGroupAsync(UserGroupCreateDto userGroupCreateDto);
    Task<UserGroupDto> GetUserGroupByIdAsync(int userGroupId);
    Task<IEnumerable<UserGroupDto>> GetAllUserGroupsAsync();
    Task UpdateUserGroupAsync(int userGroupId, UserGroupUpdateDto userGroupUpdateDto);
    Task DeleteUserGroupAsync(int userGroupId);
}
