using System;
using System.Collections.Generic;
using Lab6.Data;
namespace Lab6.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();
}
