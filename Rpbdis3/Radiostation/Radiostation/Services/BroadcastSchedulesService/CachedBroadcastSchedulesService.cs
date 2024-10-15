using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using Microsoft.EntityFrameworkCore;

namespace Radiostation.Services.BroadcastSchedulesService
{
    public class CachedBroadcastSchedulesService : ICachedBroadcastSchedulesService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CachedBroadcastSchedulesService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddBroadcastSchedules(string cacheKey, int rowNumber)
        {
            IEnumerable<BroadcastSchedule> broadcastSchedules = _context.BroadcastSchedules.Take(rowNumber).ToList();
            if (broadcastSchedules != null)
            {
                _cache.Set(cacheKey, broadcastSchedules, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<BroadcastSchedule> GetBroadcastSchedules(int rowNumber)
        {
            return _context.BroadcastSchedules
                           .Include(bs => bs.Employee) // Загрузка связанных данных Employee
                           .Include(bs => bs.Record)    // Загрузка связанных данных Record
                           .Take(rowNumber)
                           .ToList();
        }


        public IEnumerable<BroadcastSchedule> GetBroadcastSchedules(string cacheKey, int rowNumber)
        {
            IEnumerable<BroadcastSchedule> broadcastSchedules;
            if (!_cache.TryGetValue(cacheKey, out broadcastSchedules))
            {
                broadcastSchedules = _context.BroadcastSchedules.Take(rowNumber).ToList();
                if (broadcastSchedules != null)
                {
                    _cache.Set(cacheKey, broadcastSchedules, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(292)));
                }
            }
            return broadcastSchedules;
        }
    }
}
