using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Rpbdis2.models;
using System.IO;
using System;
using System.Collections.Generic;

using System.IO;

namespace Rpbdis2.data;

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

    public virtual DbSet<VwBroadcastSchedule> VwBroadcastSchedules { get; set; }

    public virtual DbSet<VwEmployee> VwEmployees { get; set; }

    public virtual DbSet<VwMusicArchive> VwMusicArchives { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ConfigurationBuilder builder = new();

        ///Установка пути к текущему каталогу
        builder.SetBasePath(Directory.GetCurrentDirectory());
        // получаем конфигурацию из файла appsettings.json
        builder.AddJsonFile("appsettings.json");
        // создаем конфигурацию
        IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

        /// Получаем строку подключения
        string connectionString = "";
        //Вариант для локального SQL Server
        connectionString = configuration.GetConnectionString("SQLConnection");
        _ = optionsBuilder
            .UseSqlServer(connectionString)
            .Options;
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));


    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("PK__Artists__25706B503CFCA005");

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasDefaultValue("Нет описания");
            entity.Property(e => e.Members)
                .HasMaxLength(255)
                .HasDefaultValue("Не указано");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<BroadcastSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Broadcas__9C8A5B493FFEEE48");

            entity.ToTable("BroadcastSchedule");

            entity.Property(e => e.BroadcastDate).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.BroadcastSchedules)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BroadcastSchedule_Employees");

            entity.HasOne(d => d.Record).WithMany(p => p.BroadcastSchedules)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Broadcast__Recor__4AB81AF0");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11A4C1B3B7");

            entity.Property(e => e.Education)
                .HasMaxLength(100)
                .HasDefaultValue("Не указано");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasDefaultValue("Не указано");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057EAD128ABC");

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasDefaultValue("Нет описания");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Records__FBDF78E962AD7D5D");

            entity.Property(e => e.Album)
                .HasMaxLength(100)
                .HasDefaultValue("Не указано");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Artist).WithMany(p => p.Records)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Records__ArtistI__403A8C7D");

            entity.HasOne(d => d.Genre).WithMany(p => p.Records)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Records__GenreId__412EB0B6");
        });

        modelBuilder.Entity<RecordDetail>(entity =>
        {
            entity.HasKey(e => e.RecordDetailId).HasName("PK__RecordDe__33C10B7990365B01");

            entity.HasIndex(e => e.RecordId, "UQ__RecordDe__FBDF78E882FE7B21").IsUnique();

            entity.Property(e => e.RecordingDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Record).WithOne(p => p.RecordDetail)
                .HasForeignKey<RecordDetail>(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecordDet__Recor__47DBAE45");
        });

        modelBuilder.Entity<VwBroadcastSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BroadcastSchedule");

            entity.Property(e => e.ArtistName).HasMaxLength(100);
            entity.Property(e => e.BroadcastDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.RecordTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<VwEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Employees");
            
            entity.Property(e => e.Education).HasMaxLength(100);
            entity.Property(e => e.EmployeeId).ValueGeneratedOnAdd();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
        });

        modelBuilder.Entity<VwMusicArchive>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_MusicArchive");

            entity.Property(e => e.Album).HasMaxLength(100);
            entity.Property(e => e.ArtistName).HasMaxLength(100);
            entity.Property(e => e.GenreName).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
