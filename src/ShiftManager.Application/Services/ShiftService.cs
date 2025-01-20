using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Application.DTOs.Shifts;
using ShiftManager.Application.Interfaces;
using ShiftManager.Domain.Entities;
using ShiftManager.Infrastructure.Data.UnitOfWork;

namespace ShiftManager.Infrastructure.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShiftService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            return await _unitOfWork.Shifts.GetAllAsync();
        }

        public async Task<Shift> GetShiftByIdAsync(int id)
        {
            return await _unitOfWork.Shifts.GetByIdAsync(id);
        }

        public async Task<ServiceResult<ShiftCreateDto>> AddShiftAsync(ShiftCreateDto dto)
        {
            try
            {
                bool isOverlapping = await IsShiftOverlapping(dto.EmployeeId, dto.StartDate, dto.StartTime, dto.EndTime);
                if (isOverlapping)
                {
                    return ServiceResult<ShiftCreateDto>.Failure("The shift overlaps with an existing shift for this employee.");
                }

                var shift = _mapper.Map<Shift>(dto);

                await _unitOfWork.Shifts.AddAsync(shift);
                await _unitOfWork.CompleteAsync();

                var createdShiftDto = _mapper.Map<ShiftCreateDto>(shift);

                return ServiceResult<ShiftCreateDto>.Success(createdShiftDto, "Shift added successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<ShiftCreateDto>.Failure(ex.Message, "An error occurred while adding the shift.");
            }
        }

        public async Task<ServiceResult<ShiftUpdateDto>> UpdateShiftAsync(ShiftUpdateDto dto)
        {
            bool isOverlapping = await IsShiftOverlapping(dto.EmployeeId, dto.StartDate, dto.StartTime, dto.EndTime);
            if (isOverlapping)
            {
                return ServiceResult<ShiftUpdateDto>.Failure("The shift overlaps with an existing shift for this employee.");
            }

            var shift = await _unitOfWork.Shifts.GetByIdAsync(dto.Id);
            if (shift == null)
            {
                return ServiceResult<ShiftUpdateDto>.Failure("Shift not found.");
            }

            shift.RoleId = dto.RoleId;
            shift.StartDate = dto.StartDate;
            shift.StartTime = dto.StartTime;
            shift.EndDate = dto.EndDate;
            shift.EndTime = dto.EndTime;


            await _unitOfWork.Shifts.UpdateAsync(shift);
            await _unitOfWork.CompleteAsync();

            var updatedShiftDto = _mapper.Map<ShiftUpdateDto>(shift);

            return ServiceResult<ShiftUpdateDto>.Success(updatedShiftDto, "Shift updated successfully.");
        }

        public async Task<ServiceResult> DeleteShiftAsync(int id)
        {
            try
            {
                var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
                if (shift == null)
                {
                    return ServiceResult.Failure("Shift not found", "Failed to delete shift");
                }

                await _unitOfWork.Shifts.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success("Shift deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ex.Message, "An error occurred while deleting the shift");
            }
        }

        public async Task<List<EmployeeShiftsDto>> GetShiftsForWeekAsync(DateOnly startOfWeek, DateOnly endOfWeek)
        {
            try
            {
                var employees = await _unitOfWork.Employees.GetAllAsync();

                var shifts = await _unitOfWork.Shifts
                .GetAllAsync(shift => shift.Include(s => s.Role));

                var shiftsForWeek = shifts
                    .Where(s => s.StartDate >= startOfWeek && s.StartDate <= endOfWeek)
                    .GroupBy(s => s.EmployeeId)
                    .ToList();

                var employeeDtos = new List<EmployeeShiftsDto>();

                foreach (var employee in employees)
                {
                    var employeeDto = _mapper.Map<EmployeeShiftsDto>(employee);

                    var employeeShifts = shiftsForWeek
                        .FirstOrDefault(g => g.Key == employee.Id)?
                        .Select(s => new ShiftDto
                        {
                            Id = s.Id,
                            RoleId = s.RoleId,
                            EmployeeId = s.EmployeeId,
                            StartDate = s.StartDate,
                            StartTime = s.StartTime,
                            EndDate = s.EndDate,
                            EndTime = s.EndTime,
                            Role = s.Role.Name
                        })
                        .ToList();

                    if (employeeShifts != null)
                    {
                        employeeDto.Shifts = employeeShifts;
                    }

                    employeeDtos.Add(employeeDto);
                }

                return employeeDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching shifts for the week.", ex);
            }
        }

        private async Task<bool> IsShiftOverlapping(int employeeId, DateOnly startDate, TimeOnly startTime, TimeOnly endTime)
        {
            var existingShifts = await _unitOfWork.Shifts.GetAllAsync(s => s
                .Where(x => x.EmployeeId == employeeId && x.StartDate == startDate)
                .AsQueryable());

            return existingShifts.Any(s =>
                (startTime >= s.StartTime && startTime < s.EndTime) ||
                (endTime > s.StartTime && endTime <= s.EndTime) ||
                (s.StartTime >= startTime && s.StartTime < endTime) ||
                (s.EndTime > startTime && s.EndTime <= endTime));
        }
    }
}