using ShiftManager.Domain.Entities;
using ShiftManager.Domain.Interfaces;
using ShiftManager.Infrastructure.Data.Repositories;

namespace ShiftManager.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShiftManagerDBContext _context;
        private IEmployeeRepository _employeeRepository;
        private IRoleRepository _roleRepository;
        private IRepository<Shift> _shiftRepository;

        public UnitOfWork(ShiftManagerDBContext context)
        {
            _context = context;
        }

        public IEmployeeRepository Employees => _employeeRepository ??= new EmployeeRepository(_context);
        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);
        public IRepository<Shift> Shifts => _shiftRepository ??= new Repository<Shift>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}