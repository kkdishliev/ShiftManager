using System;
using System.Collections.Generic;

namespace ShiftManager.Domain.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
