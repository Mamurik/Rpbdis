using System;
using System.Collections.Generic;
using Lab6.Data;
namespace Lab6.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Education { get; set; } = null!;

    public string Position { get; set; } = null!;

    public virtual ICollection<BroadcastSchedule> BroadcastSchedules { get; set; } = new List<BroadcastSchedule>();
}
