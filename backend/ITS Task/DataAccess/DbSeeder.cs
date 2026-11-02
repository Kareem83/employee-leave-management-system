
using ITS_Task.Models;
using ITS_Task.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITS_Task.DataAccess
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            var adminExists = await context.Users
                .AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists)
                return;

            var userName = configuration["SeedAdmin:UserName"];
            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Development Admin seed settings are missing.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "Development Admin password must have at least 12 characters.");
            }

            var accountExists = await context.Users.AnyAsync(
                u => u.Email == email || u.UserName == userName);

            if (accountExists)
            {
                throw new InvalidOperationException(
                    "Development Admin email or username already exists.");
            }

            var admin = new User
            {
                UserName = userName,
                Email = email,
                Role = UserRole.Admin,
                IsActive = true
            };

            admin.PasswordHash = passwordHasher.HashPassword(
                admin, password);

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
