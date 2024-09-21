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
        public List<Artist> GetAllArtists()
        {
            try
            {
                return _context.Artists.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении исполнителей: " + ex.Message);
                return new List<Artist>();
            }
        }

        // 3.2.2: Select filtered data from the "one" side table
        public List<Artist> GetArtistsByCondition(string condition)
        {
            try
            {
                return _context.Artists
                    .Where(a => a.Name.Contains(condition))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при поиске исполнителей: " + ex.Message);
                return new List<Artist>();
            }
        }

        // 3.2.3: Group data and calculate an aggregate result from the "many" side
        public List<GenreRecordCount> GetRecordsCountByGenre()
        {
            try
            {
                return _context.Records
                    .GroupBy(r => r.GenreId)
                    .Select(g => new GenreRecordCount
                    {
                        GenreId = g.Key,
                        RecordCount = g.Count()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении количества записей по жанрам: " + ex.Message);
                return new List<GenreRecordCount>();
            }
        }

        public class GenreRecordCount
        {
            public int GenreId { get; set; }
            public int RecordCount { get; set; }
        }

        // 3.2.4: Select data from two fields of two related tables (one-to-many)
        public List<ArtistRecord> GetArtistRecordTitles()
        {
            try
            {
                return _context.Records
                    .Include(r => r.Artist)
                    .Select(r => new ArtistRecord
                    {
                        ArtistName = r.Artist.Name,
                        RecordTitle = r.Title
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении названий записей и имен исполнителей: " + ex.Message);
                return new List<ArtistRecord>();
            }
        }

        public class ArtistRecord
        {
            public string ArtistName { get; set; }
            public string RecordTitle { get; set; }
        }

        // 3.2.5: Select data from two related tables filtered by a condition
        public List<ArtistRecord> GetFilteredArtistRecordTitles(string condition)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Artist)
                    .Where(r => r.Title.Contains(condition))
                    .Select(r => new ArtistRecord
                    {
                        ArtistName = r.Artist.Name,
                        RecordTitle = r.Title
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при фильтрации названий записей: " + ex.Message);
                return new List<ArtistRecord>();
            }
        }

        // 3.2.6: Insert data into the "one" side table
        public void AddArtist(Artist artist)
        {
            try
            {
                _context.Artists.Add(artist);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при добавлении исполнителя: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при добавлении исполнителя: " + ex.Message);
            }
        }

        // 3.2.7: Insert data into the "many" side table
        public void AddRecord(Record record)
        {
            try
            {
                // Проверка на существование исполнителя
                var artistExists = _context.Artists.Any(a => a.ArtistId == record.ArtistId);
                if (!artistExists)
                {
                    throw new InvalidOperationException("Исполнитель с указанным ID не существует.");
                }

                // Проверка на существование жанра
                var genreExists = _context.Genres.Any(g => g.GenreId == record.GenreId);
                if (!genreExists)
                {
                    throw new InvalidOperationException("Жанр с указанным ID не существует.");
                }

                _context.Records.Add(record);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при добавлении записи: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при добавлении записи: " + ex.Message);
            }
        }

        // 3.2.8: Delete data from the "one" side table
        public void DeleteArtist(int artistId)
        {
            try
            {
                // Найти исполнителя с включенными связанными записями
                var artist = _context.Artists
                    .Include(a => a.Records)
                    .ThenInclude(r => r.BroadcastSchedules) // Включаем связанные расписания трансляций
                    .Include(a => a.Records)
                    .ThenInclude(r => r.RecordDetail) // Включаем связанные детали записей
                    .FirstOrDefault(a => a.ArtistId == artistId);

                if (artist != null)
                {
                    // Удаляем связанные расписания трансляций и детали для каждой записи исполнителя
                    foreach (var record in artist.Records)
                    {
                        // Удаляем расписания трансляций
                        if (record.BroadcastSchedules.Any())
                        {
                            _context.BroadcastSchedules.RemoveRange(record.BroadcastSchedules);
                        }

                        // Удаляем детали записи
                        if (record.RecordDetail != null)
                        {
                            _context.RecordDetails.Remove(record.RecordDetail);
                        }
                    }

                    // Удаляем записи исполнителя
                    _context.Records.RemoveRange(artist.Records);

                    // Удаляем самого исполнителя
                    _context.Artists.Remove(artist);
                    _context.SaveChanges();

                    Console.WriteLine("Исполнитель успешно удалён!");
                }
                else
                {
                    Console.WriteLine("Исполнитель не найден.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при удалении исполнителя: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении исполнителя: " + ex.Message);
            }
        }



        public void DeleteRecord(int recordId)
        {
            try
            {
                // Найти запись с включенными связанными деталями и записями трансляций
                var record = _context.Records
                    .Include(r => r.RecordDetail) // Включаем детали записи
                    .Include(r => r.BroadcastSchedules) // Включаем расписания трансляций
                    .FirstOrDefault(r => r.RecordId == recordId);

                if (record != null)
                {
                    // Удаляем связанные расписания трансляций
                    if (record.BroadcastSchedules.Any())
                    {
                        _context.BroadcastSchedules.RemoveRange(record.BroadcastSchedules);
                    }

                    // Удаляем детали записи
                    if (record.RecordDetail != null)
                    {
                        _context.RecordDetails.Remove(record.RecordDetail);
                    }

                    // Удаляем саму запись
                    _context.Records.Remove(record);
                    _context.SaveChanges();

                    Console.WriteLine("Запись успешно удалена!");
                }
                else
                {
                    Console.WriteLine("Запись не найдена.");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при удалении записи: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении записи: " + ex.Message);
            }
        }



        // 3.2.10: Update records based on a condition
        public void UpdateRecordsByCondition(string oldTitle, string newTitle)
        {
            try
            {
                var records = _context.Records
                    .Where(r => r.Title == oldTitle)
                    .ToList();

                foreach (var record in records)
                {
                    record.Title = newTitle;
                }

                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при обновлении записей: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обновлении записей: " + ex.Message);
            }
        }
    }
}