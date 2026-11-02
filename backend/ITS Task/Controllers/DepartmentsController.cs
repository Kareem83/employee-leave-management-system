
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITS_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _departmentService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            return department == null ? NotFound() : Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateDepartmentDto dto)
        {
            try
            {
                var department = await _departmentService.AddAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = department.Id },
                    department);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, UpdateDepartmentDto dto)
        {
            try
            {
                var department = await _departmentService.UpdateAsync(id, dto);

                return department == null ? NotFound() : Ok(department);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteAsync(id);

            return result switch
            {
                "Success" => NoContent(),
                "NotFound" => NotFound(),
                "HasEmployees" => Conflict(
                    "Cannot delete a department assigned to employees."),
                _ => StatusCode(500)
            };
        }
    }
}
