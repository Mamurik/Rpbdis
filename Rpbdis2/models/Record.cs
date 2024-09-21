using System;
using System.Collections.Generic;

namespace Rpbdis2.models;

public partial class Record
{
    public int RecordId { get; set; }

    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }

    public string Album { get; set; } = null!;

    public int? Year { get; set; }

    public int GenreId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual ICollection<BroadcastSchedule> BroadcastSchedules { get; set; } = new List<BroadcastSchedule>();

    public virtual Genre Genre { get; set; } = null!;
    public virtual RecordDetail? RecordDetail { get; set; }
}
