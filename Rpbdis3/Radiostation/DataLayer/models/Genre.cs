﻿using System;
using System.Collections.Generic;
using DataLayer.Data;
namespace DataLayer.models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();
}
