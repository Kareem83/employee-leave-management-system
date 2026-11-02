
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using ITS_Task.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ITS_Task.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task UpdateAsync(User user)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountActiveAdminsAsync()
        {
            return await _context.Users.CountAsync(
                u => u.Role == UserRole.Admin && u.IsActive);
        }

    }
}
