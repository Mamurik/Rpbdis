using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Radiostation.Services.RecordDetailsService
{
    public class CacheRecordDetailsService : ICacheRecordDetailsService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CacheRecordDetailsService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddRecordDetails(string cacheKey, int rowNumber)
        {
            IEnumerable<RecordDetail> recordDetails = _context.RecordDetails
                .Include(rd => rd.Record) // Загрузка связанных данных Record
                .Take(rowNumber)
                .ToList();

            if (recordDetails != null)
            {
                _cache.Set(cacheKey, recordDetails, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<RecordDetail> GetRecordDetails(int rowNumber)
        {
            return _context.RecordDetails
                .Include(rd => rd.Record) // Загрузка связанных данных Record
                .Take(rowNumber)
                .ToList();
        }

        public IEnumerable<RecordDetail> GetRecordDetails(string cacheKey, int rowNumber)
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<RecordDetail> recordDetails))
            {
                recordDetails = _context.RecordDetails
                    .Include(rd => rd.Record) // Загрузка связанных данных Record
                    .Take(rowNumber)
                    .ToList();

                if (recordDetails != null)
                {
                    _cache.Set(cacheKey, recordDetails, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                    });
                }
            }
            return recordDetails;
        }
    }
}
