using ShiftManager.Application.DTOs.Shifts;

namespace ShiftManager.Application.DTOs.Employees
{
    public class EmployeeShiftsDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<ShiftDto>? Shifts { get; set; }
    }
}