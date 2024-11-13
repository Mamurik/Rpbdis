using RadiostationWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    internal class TestDataHelper
    {
        public static List<Genre> GetFakeGenresList()
        {
            return new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Rock", Description = "A genre characterized by a strong rhythm and often simple melodies." },
                new Genre { GenreId = 2, Name = "Pop", Description = "A genre that emphasizes catchy melodies and simple lyrics." },
                new Genre { GenreId = 3, Name = "Jazz", Description = "A genre known for its swing and blue notes, call and response vocals, polyrhythms, and improvisation." }
            };
        }

        public static List<Artist> GetFakeArtistsList()
        {
            return new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "The Beatles", Members = "John Lennon, Paul McCartney, George Harrison, Ringo Starr", Description = "An English rock band formed in Liverpool." },
                new Artist { ArtistId = 2, Name = "Taylor Swift", Members = "Taylor Swift", Description = "An American singer-songwriter known for narrative songs." },
                new Artist { ArtistId = 3, Name = "Miles Davis", Members = "Miles Davis", Description = "An American jazz trumpeter, bandleader, and composer." }
            };
        }

        public static List<RadiostationWeb.Models.Record> GetFakeRecordsList() 
        {
            var genres = GetFakeGenresList();
            var artists = GetFakeArtistsList();

            return new List<RadiostationWeb.Models.Record>
            {
                new RadiostationWeb.Models.Record { RecordId = 1, Title = "Abbey Road", ArtistId = 1, Album = "Abbey Road", Year = 1969, GenreId = 1, Artist = artists.First(a => a.ArtistId == 1), Genre = genres.First(g => g.GenreId == 1) },
                new RadiostationWeb.Models.Record { RecordId = 2, Title = "1989", ArtistId = 2, Album = "1989", Year = 2014, GenreId = 2, Artist = artists.First(a => a.ArtistId == 2), Genre = genres.First(g => g.GenreId == 2) },
                new RadiostationWeb.Models.Record { RecordId = 3, Title = "Kind of Blue", ArtistId = 3, Album = "Kind of Blue", Year = 1959, GenreId = 3, Artist = artists.First(a => a.ArtistId == 3), Genre = genres.First(g => g.GenreId == 3) }
            };
        }

        public static List<RecordDetail> GetFakeRecordDetailsList()
        {
            var records = GetFakeRecordsList();

            return new List<RecordDetail>
            {
                new RecordDetail { RecordDetailId = 1, RecordId = 1, RecordingDate = new DateOnly(1969, 7, 20), Duration = new TimeOnly(0, 42, 33), Rating = 5, Record = records.First(r => r.RecordId == 1) },
                new RecordDetail { RecordDetailId = 2, RecordId = 2, RecordingDate = new DateOnly(2014, 8, 18), Duration = new TimeOnly(0, 48, 33), Rating = 4, Record = records.First(r => r.RecordId == 2) },
                new RecordDetail { RecordDetailId = 3, RecordId = 3, RecordingDate = new DateOnly(1959, 8, 17), Duration = new TimeOnly(0, 45, 29), Rating = 5, Record = records.First(r => r.RecordId == 3) }
            };
        }

        public static List<BroadcastSchedule> GetFakeBroadcastSchedulesList()
        {
            var records = GetFakeRecordsList();
            var employees = GetFakeEmployeesList(); 

            return new List<BroadcastSchedule>
            {
                new BroadcastSchedule { ScheduleId = 1, BroadcastDate = DateTime.Now.AddDays(1), EmployeeId = 1, RecordId = 1, Employee = employees.First(e => e.EmployeeId == 1), Record = records.First(r => r.RecordId == 1) },
                new BroadcastSchedule { ScheduleId = 2, BroadcastDate = DateTime.Now.AddDays(2), EmployeeId = 2, RecordId = 2, Employee = employees.First(e => e.EmployeeId == 2), Record = records.First(r => r.RecordId == 2) },
                new BroadcastSchedule { ScheduleId = 3, BroadcastDate = DateTime.Now.AddDays(3), EmployeeId = 3, RecordId = 3, Employee = employees.First(e => e.EmployeeId == 3), Record = records.First(r => r.RecordId == 3) }
            };
        }

        public static List<Employee> GetFakeEmployeesList()
        {
            return new List<Employee>
            {
                new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" },
                new Employee { EmployeeId = 2, FullName = "Анна Иванова", Education = "Среднее", Position = "Звукорежиссер" },
                new Employee { EmployeeId = 3, FullName = "Дмитрий Смирнов", Education = "Высшее", Position = "Продюсер" }
            };
        }
    }
}