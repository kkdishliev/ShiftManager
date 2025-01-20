using ShiftManager.Domain.Entities;

namespace ShiftManager.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        IQueryable<Role> Query();
        Task<List<Role>> GetByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Role>> GetRolesByEmployeeIdAsync(int employeeId);
    }
}