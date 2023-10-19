//using Auth.Exercise.Two.Entities;
//using Auth.Exercise.Two.Persistence;
using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services
{
	public interface IRoleService
	{
		Task<List<ApplicationRole>> GetRolesAsync();
		Task<ApplicationRole> GetRoleByIdAsync(int id);
		Task CreateRoleAsync(ApplicationRole Role);
		Task UpdateRoleAsync(ApplicationRole Role);
		Task DeleteRoleAsync(int id);
	}

	public class RoleService : IRoleService
	{
		private readonly ApplicationDbContext _context;

		public RoleService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ApplicationRole>> GetRolesAsync()
		{
			return await _context.Roles.ToListAsync();
		}

		public async Task<ApplicationRole> GetRoleByIdAsync(int id)
		{
			return await _context.Roles.FindAsync(id);
		}

		public async Task CreateRoleAsync(ApplicationRole Role)
		{
			_context.Roles.Add(Role);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateRoleAsync(ApplicationRole Role)
		{
			_context.Entry(Role).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteRoleAsync(int id)
		{
			var Role = await _context.Roles.FindAsync(id);
			if (Role != null)
			{
				_context.Roles.Remove(Role);
				await _context.SaveChangesAsync();
			}
		}
	}
}