
using ITS_Task.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class TeamController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public TeamController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("members")]
        public async Task<IActionResult> GetMembers()
        {
            var claim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(claim, out var userId))
                return Unauthorized();

            var manager = await _employeeService
                .GetMyProfileAsync(userId);

            if (manager == null)
                return NotFound("Manager profile not found.");

            var result = await _employeeService
                .GetMyTeamAsync(userId);

            return Ok(result);
        }
    }
}
