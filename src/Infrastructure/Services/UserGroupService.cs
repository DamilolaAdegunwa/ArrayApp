using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Infrastructure.Services.Interfaces;
namespace ArrayApp.Infrastructure.Services;
public class UserGroupService : IUserGroupService
{
    public Task<UserGroupDto> CreateUserGroupAsync(UserGroupCreateDto userGroupCreateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserGroupAsync(int userGroupId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserGroupDto>> GetAllUserGroupsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserGroupDto> GetUserGroupByIdAsync(int userGroupId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserGroupAsync(int userGroupId, UserGroupUpdateDto userGroupUpdateDto)
    {
        throw new NotImplementedException();
    }
}
