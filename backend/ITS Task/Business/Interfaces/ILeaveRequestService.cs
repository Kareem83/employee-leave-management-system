
using ITS_Task.Business.DTOs;

namespace ITS_Task.Business.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDto> SubmitAsync(
            int userId,
            CreateLeaveRequestDto dto);
    

    Task<string> ApproveAsync(
    int requestId, int currentUserId, bool isAdmin);

        Task<string> RejectAsync(
            int requestId,
            int currentUserId,
            bool isAdmin,
            string rejectionReason);

        Task<List<LeaveRequestDto>> GetMyRequestsAsync(int userId);

        Task<List<LeaveRequestDto>> GetTeamRequestsAsync(int userId);

        Task<string> CancelAsync(int requestId, int userId);
        Task<List<LeaveRequestDto>> GetAllRequestsAsync();
    }


}
