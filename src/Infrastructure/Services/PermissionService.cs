using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services
{
	public interface IPermissionService
	{
		Task<List<Permission>> GetPermissionsAsync();
		Task<Permission> GetPermissionByIdAsync(int id);
		Task CreatePermissionAsync(Permission permission);
		Task UpdatePermissionAsync(Permission permission);
		Task DeletePermissionAsync(int id);
	}

	public class PermissionService : IPermissionService
	{
		private readonly ApplicationDbContext _context;

		public PermissionService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Permission>> GetPermissionsAsync()
		{
			
			return await _context.Permissions.ToListAsync();
		}

		public async Task<Permission> GetPermissionByIdAsync(int id)
		{
			return await _context.Permissions.FindAsync(id);
		}

		public async Task CreatePermissionAsync(Permission permission)
		{
			_context.Permissions.Add(permission);
			await _context.SaveChangesAsync();
		}

		public async Task UpdatePermissionAsync(Permission permission)
		{
			_context.Entry(permission).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeletePermissionAsync(int id)
		{
			var permission = await _context.Permissions.FindAsync(id);
			if (permission != null)
			{
				_context.Permissions.Remove(permission);
				await _context.SaveChangesAsync();
			}
		}
	}
}