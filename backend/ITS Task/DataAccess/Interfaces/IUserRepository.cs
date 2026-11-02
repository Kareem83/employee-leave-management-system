
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameAsync(string userName);

        Task<User?> GetByIdAsync(int id);

        Task UpdateAsync(User user);

        Task<int> CountActiveAdminsAsync();
    }
}
