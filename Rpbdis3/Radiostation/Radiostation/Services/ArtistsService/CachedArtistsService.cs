using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using Radiostation.Services.ArtistsService;

namespace Radiostation.Services.ArtistsService
{
    public class CachedArtistsService : ICachedArtistsService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CachedArtistsService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddArtists(string cacheKey, int rowNumber)
        {
            IEnumerable<Artist> artists = _context.Artists.Take(rowNumber).ToList();
            if (artists != null)
            {
                _cache.Set(cacheKey, artists, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<Artist> GetArtists(int rowNumber)
        {
            return _context.Artists.Take(rowNumber).ToList();
        }

        public IEnumerable<Artist> GetArtists(string cacheKey, int rowNumber)
        {
            IEnumerable<Artist> artists;
            if (!_cache.TryGetValue(cacheKey, out artists))
            {
                artists = _context.Artists.Take(rowNumber).ToList();
                if (artists != null)
                {
                    _cache.Set(cacheKey, artists, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(292)));
                }
            }
            return artists;
        }
    }
}