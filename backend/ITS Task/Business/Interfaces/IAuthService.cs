using ITS_Task.Business.DTOs;

namespace ITS_Task.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    }
}