using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using System.Collections.Generic;
using System.Linq;

namespace Radiostation.Services.GenresService
{
    public class CacheGenresService : ICacheGenresService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CacheGenresService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddGenres(string cacheKey, int rowNumber)
        {
            IEnumerable<Genre> genres = _context.Genres.Take(rowNumber).ToList();
            if (genres != null)
            {
                _cache.Set(cacheKey, genres, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<Genre> GetGenres(int rowNumber)
        {
            return _context.Genres
                           .Take(rowNumber)
                           .ToList();
        }

        public IEnumerable<Genre> GetGenres(string cacheKey, int rowNumber)
        {
            IEnumerable<Genre> genres;
            if (!_cache.TryGetValue(cacheKey, out genres))
            {
                genres = _context.Genres.Take(rowNumber).ToList();
                if (genres != null)
                {
                    _cache.Set(cacheKey, genres, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(292)));
                }
            }
            return genres;
        }
    }
}
