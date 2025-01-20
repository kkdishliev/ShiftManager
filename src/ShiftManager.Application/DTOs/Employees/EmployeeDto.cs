using ShiftManager.Application.DTOs.Roles;

namespace ShiftManager.Application.DTOs.Employees
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<RoleDto>? Roles { get; set; }
    }
}