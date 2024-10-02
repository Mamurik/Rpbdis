using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rpbdis2.models;

namespace Rpbdis2.data
{
    public class DataOperations
    {
        private readonly RadioStationDbContext _context;

        public DataOperations(RadioStationDbContext context)
        {
            _context = context;
        }

        // 3.2.1: Select all data from the "one" side table
        public List<Artist> GetAllArtists() => ExecuteWithLogging(() => _context.Artists.ToList());

        // 3.2.2: Select filtered data from the "one" side table
        public List<Artist> GetArtistsByCondition(string condition) =>
            ExecuteWithLogging(() => _context.Artists.Where(a => a.Name.Contains(condition)).ToList());

        // 3.2.3: Group data and calculate an aggregate result from the "many" side
        public List<GenreRecordCount> GetRecordsCountByGenre() =>
            ExecuteWithLogging(() => _context.Records
                .GroupBy(r => r.GenreId)
                .Select(g => new GenreRecordCount
                {
                    GenreId = g.Key,
                    RecordCount = g.Count()
                })
                .ToList());

        public class GenreRecordCount
        {
            public int GenreId { get; set; }
            public int RecordCount { get; set; }
        }

        // 3.2.4: Select data from two fields of two related tables (one-to-many)
        public List<ArtistRecord> GetArtistRecordTitles() =>
            ExecuteWithLogging(() => _context.Records
                .Include(r => r.Artist)
                .Select(r => new ArtistRecord
                {
                    ArtistName = r.Artist.Name,
                    RecordTitle = r.Title
                })
                .ToList());

        public class ArtistRecord
        {
            public string ArtistName { get; set; }
            public string RecordTitle { get; set; }
        }

        // 3.2.5: Select data from two related tables filtered by a condition
        public List<ArtistRecord> GetFilteredArtistRecordTitles(string condition) =>
            ExecuteWithLogging(() => _context.Records
                .Include(r => r.Artist)
                .Where(r => r.Title.Contains(condition))
                .Select(r => new ArtistRecord
                {
                    ArtistName = r.Artist.Name,
                    RecordTitle = r.Title
                })
                .ToList());

        // 3.2.6: Insert data into the "one" side table
        public void AddArtist(Artist artist) =>
            ExecuteWithLogging(() =>
            {
                _context.Artists.Add(artist);
                _context.SaveChanges();
            });

        // 3.2.7: Insert data into the "many" side table
        public void AddRecord(Record record) =>
            ExecuteWithLogging(() =>
            {
                if (!_context.Artists.Any(a => a.ArtistId == record.ArtistId))
                    throw new InvalidOperationException("Исполнитель с указанным ID не существует.");

                if (!_context.Genres.Any(g => g.GenreId == record.GenreId))
                    throw new InvalidOperationException("Жанр с указанным ID не существует.");

                _context.Records.Add(record);
                _context.SaveChanges();
            });

        // 3.2.8: Delete data from the "one" side table
        public void DeleteArtist(int artistId) =>
            ExecuteWithLogging(() =>
            {
                var artist = _context.Artists
                    .Include(a => a.Records)
                        .ThenInclude(r => r.BroadcastSchedules)
                    .Include(a => a.Records)
                        .ThenInclude(r => r.RecordDetail)
                    .FirstOrDefault(a => a.ArtistId == artistId);

                if (artist != null)
                {
                    _context.BroadcastSchedules.RemoveRange(artist.Records.SelectMany(r => r.BroadcastSchedules));
                    _context.RecordDetails.RemoveRange(artist.Records.Select(r => r.RecordDetail).Where(rd => rd != null));
                    _context.Records.RemoveRange(artist.Records);
                    _context.Artists.Remove(artist);
                    _context.SaveChanges();

                    Console.WriteLine("Исполнитель успешно удалён!");
                }
                else
                {
                    Console.WriteLine("Исполнитель не найден.");
                }
            });

        public void DeleteRecord(int recordId) =>
            ExecuteWithLogging(() =>
            {
                var record = _context.Records
                    .Include(r => r.RecordDetail)
                    .Include(r => r.BroadcastSchedules)
                    .FirstOrDefault(r => r.RecordId == recordId);

                if (record != null)
                {
                    _context.BroadcastSchedules.RemoveRange(record.BroadcastSchedules);
                    if (record.RecordDetail != null)
                    {
                        _context.RecordDetails.Remove(record.RecordDetail);
                    }

                    _context.Records.Remove(record);
                    _context.SaveChanges();

                    Console.WriteLine("Запись успешно удалена!");
                }
                else
                {
                    Console.WriteLine("Запись не найдена.");
                }
            });

        // 3.2.10: Update records based on a condition
        public void UpdateRecordsByCondition(string oldTitle, string newTitle) =>
            ExecuteWithLogging(() =>
            {
                var records = _context.Records
                    .Where(r => r.Title == oldTitle)
                    .ToList();

                records.ForEach(record => record.Title = newTitle);
                _context.SaveChanges();
            });

        private T ExecuteWithLogging<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return default;
            }
        }

        private void ExecuteWithLogging(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}