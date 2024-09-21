using System;
using System.Collections.Generic;

namespace Rpbdis2.models;

public partial class VwBroadcastSchedule
{
    public int ScheduleId { get; set; }

    public DateTime BroadcastDate { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string RecordTitle { get; set; } = null!;

    public string ArtistName { get; set; } = null!;
}
