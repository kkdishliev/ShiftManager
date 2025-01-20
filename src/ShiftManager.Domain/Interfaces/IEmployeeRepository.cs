using ShiftManager.Domain.Entities;

namespace ShiftManager.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        IQueryable<Employee> Query();
        Task<Employee> GetEmployeeWithRolesAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAsync(Func<Employee, bool> predicate);
        Task<List<Employee>> GetAllEmployeesAsync();
    }
}