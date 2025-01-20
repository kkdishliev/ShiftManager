using Microsoft.EntityFrameworkCore;
using ShiftManager.Domain.Entities;
using ShiftManager.Domain.Interfaces;

namespace ShiftManager.Infrastructure.Data.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly ShiftManagerDBContext _context;

        public RoleRepository(ShiftManagerDBContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Role> Query()
        {
            return _context.Roles.AsQueryable();
        }

        public async Task<List<Role>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Roles
                .Where(role => ids.Contains(role.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesByEmployeeIdAsync(int employeeId)
        {
            return await _context.Employees
                .Where(e => e.Id == employeeId)
                .SelectMany(e => e.Roles)
                .ToListAsync();
        }
    }
}