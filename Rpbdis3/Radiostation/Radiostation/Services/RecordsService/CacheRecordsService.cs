using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Radiostation.Services.RecordsService
{
    public class CacheRecordsService : ICacheRecordsService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CacheRecordsService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddRecords(string cacheKey, int rowNumber)
        {
            IEnumerable<Record> records = _context.Records
                .Include(r => r.Artist)        // Загрузка связанных данных Artist
                .Include(r => r.Genre)         // Загрузка связанных данных Genre
                .Include(r => r.RecordDetail)   // Загрузка связанных данных RecordDetail
                .Take(rowNumber)
                .ToList();

            if (records != null)
            {
                _cache.Set(cacheKey, records, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<Record> GetRecords(int rowNumber)
        {
            return _context.Records
                .Include(r => r.Artist)        // Загрузка связанных данных Artist
                .Include(r => r.Genre)         // Загрузка связанных данных Genre
                .Include(r => r.RecordDetail)   // Загрузка связанных данных RecordDetail
                .Take(rowNumber)
                .ToList();
        }

        public IEnumerable<Record> GetRecords(string cacheKey, int rowNumber)
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Record> records))
            {
                records = _context.Records
                    .Include(r => r.Artist)        // Загрузка связанных данных Artist
                    .Include(r => r.Genre)         // Загрузка связанных данных Genre
                    .Include(r => r.RecordDetail)   // Загрузка связанных данных RecordDetail
                    .Take(rowNumber)
                    .ToList();

                if (records != null)
                {
                    _cache.Set(cacheKey, records, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                    });
                }
            }
            return records;
        }
    }
}
