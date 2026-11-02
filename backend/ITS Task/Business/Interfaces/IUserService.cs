
using ITS_Task.Models.Enums;

namespace ITS_Task.Business.Interfaces
{
    public interface IUserService
    {
        Task<string> ChangeRoleAsync(
            int targetUserId,
            int currentUserId,
            UserRole newRole);
    }
}
