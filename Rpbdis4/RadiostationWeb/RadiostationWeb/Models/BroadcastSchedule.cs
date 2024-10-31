using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RadiostationWeb.Data;
namespace RadiostationWeb.Models;

[Table("BroadcastSchedule")]
public partial class BroadcastSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    public DateTime BroadcastDate { get; set; }

    public int EmployeeId { get; set; }

    public int RecordId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Record Record { get; set; } = null!;
}
