using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
namespace ArrayApp.Infrastructure.Services;

public interface IUserRoleService
{
    Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    Task<IEnumerable<string>> GetUsersInRoleAsync(string roleName);
}

public class UserRoleService : IUserRoleService
{
    // Inject your DbContext into your service or controller
    private readonly ApplicationDbContext _context;

    public UserRoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Example method to retrieve roles for a specific user
    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var roles = await _context.Roles
            .Where(r => userRoles.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync();

        return roles;
    }

    // Example method to retrieve users in a specific role
    public async Task<IEnumerable<string>> GetUsersInRoleAsync(string roleName)
    {
        var role = await _context.Roles
            .Where(r => r.Name == roleName)
            .FirstOrDefaultAsync();

        if (role != null)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => userRoles.Contains(u.Id))
                .Select(u => u.UserName)
                .ToListAsync();

            return users;
        }

        return Enumerable.Empty<string>();
    }

}
