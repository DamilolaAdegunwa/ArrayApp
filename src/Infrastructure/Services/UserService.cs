using ArrayApp.Domain.Entities;
using ArrayApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services
{
	public interface IUserService
	{
		Task<List<ApplicationUser>> GetUsersAsync();
		Task<ApplicationUser> GetUserByIdAsync(int id);
		Task CreateUserAsync(ApplicationUser User);
		Task UpdateUserAsync(ApplicationUser User);
		Task DeleteUserAsync(int id);
	}

	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationRole> _userManager;

		public UserService(ApplicationDbContext context, UserManager<ApplicationRole> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<List<ApplicationUser>> GetUsersAsync()
		{

			return await _context.Users.ToListAsync();
		}

		public async Task<ApplicationUser> GetUserByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task CreateUserAsync(ApplicationUser User)
		{
			_context.Users.Add(User);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateUserAsync(ApplicationUser User)
		{
			_context.Entry(User).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteUserAsync(int id)
		{
			var User = await _context.Users.FindAsync(id);
			if (User != null)
			{
				_context.Users.Remove(User);
				await _context.SaveChangesAsync();
			}
		}


	}
}