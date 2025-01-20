using ShiftManager.Domain.Entities;
using ShiftManager.Domain.Interfaces;

namespace ShiftManager.Infrastructure.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IRoleRepository Roles { get; }
        IRepository<Shift> Shifts { get; }

        Task<int> CompleteAsync();
    }
}