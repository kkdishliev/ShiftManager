using System;
using System.Collections.Generic;

namespace ShiftManager.Domain.Entities;

public partial class Shift
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int RoleId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
