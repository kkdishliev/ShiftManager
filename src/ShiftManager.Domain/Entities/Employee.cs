using System;
using System.Collections.Generic;

namespace ShiftManager.Domain.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
