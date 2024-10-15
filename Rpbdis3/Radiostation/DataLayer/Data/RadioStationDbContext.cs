using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DataLayer.models;

namespace DataLayer.Data;

public partial class RadioStationDbContext : DbContext
{
    public RadioStationDbContext()
    {
    }

    public RadioStationDbContext(DbContextOptions<RadioStationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<BroadcastSchedule> BroadcastSchedules { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<RecordDetail> RecordDetails { get; set; }

}
