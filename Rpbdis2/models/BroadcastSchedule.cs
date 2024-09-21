using System;
using System.Collections.Generic;

namespace Rpbdis2.models;

public partial class BroadcastSchedule
{
    public int ScheduleId { get; set; }

    public DateTime BroadcastDate { get; set; }

    public int EmployeeId { get; set; }

    public int RecordId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Record Record { get; set; } = null!;
}
