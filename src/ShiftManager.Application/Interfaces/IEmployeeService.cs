using ShiftManager.Application.Common;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Domain.Entities;

namespace ShiftManager.Application.Interfaces
{
    public interface IEmployeeService
    {
        public Task<EmployeeApiResponse<EmployeeDto>> GetAllEmployeesAsync(
            int start,
            int size,
            string globalFilter,
            string? filters = null,
            string? sorting = null);
        public Task<Employee> GetEmployeeByIdAsync(int id);
        public Task<ServiceResult<EmployeeDto>> AddEmployeeAsync(EmployeeDto dto);
        public Task<ServiceResult<EmployeeDto>> UpdateEmployeeAsync(EmployeeDto dto);
        public Task<ServiceResult> DeleteEmployeeAsync(int id);
        public Task<Employee> GetEmployeeWithRolesAsync(int id);
        public Task<ServiceResult<EmployeeDto>> AddRolesToEmployeeAsync(int employeeId, List<int> roleIds);
        public Task<List<EmployeeDto>> GetAllEmployeesForDropdownAsync();
    }
}