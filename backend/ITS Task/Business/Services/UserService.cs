
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using ITS_Task.Models.Enums;

namespace ITS_Task.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        public UserService(IUserRepository userRepository, IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<string> ChangeRoleAsync(
            int targetUserId,
            int currentUserId,
            UserRole newRole)
        {
            if (!Enum.IsDefined(typeof(UserRole), newRole))
                return "InvalidRole";

            if (targetUserId == currentUserId)
                return "CannotChangeOwnRole";

            var user = await _userRepository.GetByIdAsync(targetUserId);

            if (user == null)
                return "NotFound";

            if (!user.IsActive)
                return "InactiveUser";

            if (user.Role == newRole)
                return "NoChange";

            if (user.Role == UserRole.Admin &&
                newRole != UserRole.Admin)
            {
                var adminCount =
                    await _userRepository.CountActiveAdminsAsync();

                if (adminCount <= 1)
                    return "LastAdmin";
            }


            user.Role = newRole;

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = currentUserId,
                Action = $"ChangeRoleTo:{newRole}",
                EntityName = "User",
                EntityId = targetUserId,
                CreatedAt = DateTime.UtcNow
            });

            await _userRepository.UpdateAsync(user);

            return "Success";

        }
    }
}
