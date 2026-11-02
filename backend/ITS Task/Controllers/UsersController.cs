
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(
            int id,
            [FromBody] UpdateUserRoleDto dto)
        {
            if (!dto.Role.HasValue)
                return BadRequest("Role is required.");

            var currentUserIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserIdValue, out var currentUserId))
                return Unauthorized();

            var result = await _userService.ChangeRoleAsync(
                id,
                currentUserId,
                dto.Role.Value);

            return result switch
            {
                "Success" => Ok("Role updated successfully."),
                "NoChange" => Ok("User already has this role."),
                "NotFound" => NotFound("User not found."),
                "InvalidRole" => BadRequest("Invalid role."),
                "InactiveUser" => BadRequest("User is inactive."),
                "CannotChangeOwnRole" =>
                    BadRequest("You cannot change your own role."),
                "LastAdmin" =>
                    BadRequest("Cannot remove the last active admin."),
                _ => StatusCode(500, "Unexpected error.")
            };
        }
    }
}
