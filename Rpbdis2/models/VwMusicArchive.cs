using System;
using System.Collections.Generic;

namespace Rpbdis2.models;

public partial class VwMusicArchive
{
    public int RecordId { get; set; }

    public string Title { get; set; } = null!;

    public string ArtistName { get; set; } = null!;

    public string GenreName { get; set; } = null!;

    public string Album { get; set; } = null!;

    public int? Year { get; set; }
}
