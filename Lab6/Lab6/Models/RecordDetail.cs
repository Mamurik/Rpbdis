using System;
using System.Collections.Generic;
using Lab6.Data;
namespace Lab6.Models;

public partial class RecordDetail
{
    public int RecordDetailId { get; set; }

    public int RecordId { get; set; }

    public DateOnly RecordingDate { get; set; }

    public TimeOnly Duration { get; set; }

    public int Rating { get; set; }

    public virtual Record Record { get; set; } = null!;
}
