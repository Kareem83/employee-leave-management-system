
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestsController(ILeaveRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(
            [FromBody] CreateLeaveRequestDto dto)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                return Unauthorized();

            try
            {
                var result = await _service.SubmitAsync(userId, dto);

                return StatusCode(201, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var result = await _service.ApproveAsync(
                id, userId, isAdmin);

            return ReviewResponse(result);
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Reject(
            int id,
            [FromBody] RejectLeaveRequestDto dto)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var result = await _service.RejectAsync(
                id, userId, isAdmin, dto.RejectionReason);

            return ReviewResponse(result);
        }

        private IActionResult ReviewResponse(string result)
        {
            return result switch
            {
                "Success" => Ok("Request reviewed successfully."),
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                "AlreadyReviewed" => Conflict(
                    "Request is no longer pending."),
                "InsufficientBalance" => Conflict(
                    "Insufficient leave balance."),
                "BalanceNotFound" => Conflict(
                    "Leave balance not found."),
                "InactiveEmployee" => Conflict(
                    "Employee is inactive."),
                "ReasonRequired" => BadRequest(
                    "Rejection reason is required."),
                _ => StatusCode(500)
            };
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            try
            {
                return Ok(await _service.GetMyRequestsAsync(userId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("team")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTeamRequests()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            try
            {
                return Ok(await _service.GetTeamRequestsAsync(userId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var result = await _service.CancelAsync(id, userId);

            return result switch
            {
                "Success" => Ok("Leave request cancelled."),
                "NotFound" => NotFound(),
                "Forbidden" => Forbid(),
                "CannotCancel" => Conflict("Only pending requests can be cancelled."),
                "EmployeeNotFound" => BadRequest("Employee not found."),
                _ => StatusCode(500)
            };
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRequests()
        {
            var result = await _service.GetAllRequestsAsync();

            return Ok(result);
        }

    }
}
