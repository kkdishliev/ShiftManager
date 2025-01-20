using ShiftManager.Domain.Entities;
using ShiftManager.Domain.Interfaces;

namespace ShiftManager.Infrastructure.Data.Repositories
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(ShiftManagerDBContext context) : base(context)
        {
        }
    }
}