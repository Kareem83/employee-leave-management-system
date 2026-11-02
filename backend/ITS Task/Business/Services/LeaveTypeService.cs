
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;

namespace ITS_Task.Business.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveTypeRepository _repository;

        public LeaveTypeService(ILeaveTypeRepository repository)
        {
            _repository = repository;
        }
        //MapToDto دي Method إحنا عاملينها بنفسنا داخل LeaveTypeService
        //وظيفتها تحويل LeaveType (Entity) إلى LeaveTypeDto علشان نرجع البيانات المطلوبة بس للـFrontend.
        public async Task<List<LeaveTypeDto>> GetAllAsync()
        {
            var leaveTypes = await _repository.GetAllAsync();

            return leaveTypes.Select(MapToDto).ToList();
        }

        public async Task<LeaveTypeDto?> GetByIdAsync(int id)
        {
            var leaveType = await _repository.GetByIdAsync(id);

            if (leaveType == null || !leaveType.IsActive)
                return null;

            return MapToDto(leaveType);
        }

        public async Task<LeaveTypeDto> AddAsync(CreateLeaveTypeDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Leave type name is required.");

            var existing = await _repository.GetByNameAsync(name);

            if (existing != null)
                throw new InvalidOperationException(
                    "Leave type already exists.");

            var leaveType = new LeaveType
            {
                Name = name,
                DefaultDays = dto.DefaultDays,
                IsActive = true
            };

            await _repository.AddAsync(leaveType);

            return MapToDto(leaveType);
        }

        public async Task<LeaveTypeDto?> UpdateAsync(
            int id, UpdateLeaveTypeDto dto)
        {
            var leaveType = await _repository.GetByIdAsync(id);

            if (leaveType == null || !leaveType.IsActive)
                return null;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Leave type name is required.");

            var existing = await _repository.GetByNameAsync(name);

            if (existing != null && existing.Id != id)
                throw new InvalidOperationException(
                    "Leave type already exists.");

            leaveType.Name = name;
            leaveType.DefaultDays = dto.DefaultDays;

            await _repository.UpdateAsync(leaveType);

            return MapToDto(leaveType);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var leaveType = await _repository.GetByIdAsync(id);

            if (leaveType == null || !leaveType.IsActive)
                return false;

            leaveType.IsActive = false;

            await _repository.UpdateAsync(leaveType);

            return true;
        }

        private static LeaveTypeDto MapToDto(LeaveType leaveType)
        {
            return new LeaveTypeDto
            {
                Id = leaveType.Id,
                Name = leaveType.Name,
                DefaultDays = leaveType.DefaultDays,
                IsActive = leaveType.IsActive
            };
        }
    }
}
