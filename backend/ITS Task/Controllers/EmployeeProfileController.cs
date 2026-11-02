
using ITS_Task.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_Task.Controllers
{
    [ApiController]
    [Route("api/employee-profile")]
    [Authorize]
    public class EmployeeProfileController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeProfileController(
            IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var profile = await _employeeService
                .GetMyProfileAsync(userId);

            if (profile == null)
                return NotFound("Employee profile not found.");

            return Ok(profile);
        }
    }
}
