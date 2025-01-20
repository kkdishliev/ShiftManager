using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Application.DTOs.Shifts;
using ShiftManager.Domain.Entities;

namespace ShiftManager.Application.Interfaces
{
    public interface IShiftService
    {
        public Task<IEnumerable<Shift>> GetAllShiftsAsync();
        public Task<Shift> GetShiftByIdAsync(int id);
        public Task<ServiceResult<ShiftCreateDto>> AddShiftAsync(ShiftCreateDto dto);
        public Task<ServiceResult<ShiftUpdateDto>> UpdateShiftAsync(ShiftUpdateDto dto);
        public Task<ServiceResult> DeleteShiftAsync(int id);
        public Task<List<EmployeeShiftsDto>> GetShiftsForWeekAsync(DateOnly startOfWeek, DateOnly endOfWeek);
    }
}