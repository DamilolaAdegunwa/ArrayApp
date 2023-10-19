using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services
{
	public interface IUserPermissionService
	{
		Task<List<UserPermission>> GetUserPermissionsAsync();
		Task<UserPermission> GetUserPermissionByIdAsync(int id);
		Task CreateUserPermissionAsync(UserPermission UserPermission);
		Task UpdateUserPermissionAsync(UserPermission UserPermission);
		Task DeleteUserPermissionAsync(int id);
	}

	public class UserPermissionService : IUserPermissionService
	{
		private readonly ApplicationDbContext _context;

		public UserPermissionService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<UserPermission>> GetUserPermissionsAsync()
		{

			return await _context.UserPermissions.ToListAsync();
		}

		public async Task<UserPermission> GetUserPermissionByIdAsync(int id)
		{
			return await _context.UserPermissions.FindAsync(id);
		}

		public async Task CreateUserPermissionAsync(UserPermission UserPermission)
		{
			_context.UserPermissions.Add(UserPermission);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateUserPermissionAsync(UserPermission UserPermission)
		{
			_context.Entry(UserPermission).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteUserPermissionAsync(int id)
		{
			var UserPermission = await _context.UserPermissions.FindAsync(id);
			if (UserPermission != null)
			{
				_context.UserPermissions.Remove(UserPermission);
				await _context.SaveChangesAsync();
			}
		}
	}
}