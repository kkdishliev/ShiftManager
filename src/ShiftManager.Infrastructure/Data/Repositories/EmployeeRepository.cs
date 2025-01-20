namespace ShiftManager.Infrastructure.Data.Repositories
{
    using ShiftManager.Domain.Entities;
    using ShiftManager.Domain.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ShiftManagerDBContext context) : base(context)
        {
        }

        public IQueryable<Employee> Query()
        {
            return _context.Employees.AsQueryable();
        }

        public async Task<Employee> GetEmployeeWithRolesAsync(int id)
        {
            return await _context.Employees
                                 .Include(e => e.Roles) 
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<int> CountAsync(Func<Employee, bool> predicate)
        {
            return await Task.FromResult(_context.Employees.Where(predicate).Count());
        }
        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();  
        }
    }
}