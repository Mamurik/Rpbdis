using DataLayer.models;

namespace Radiostation.Services.ArtistsService
{
    public interface ICachedArtistsService
    {
        public IEnumerable<Artist> GetArtists(int rowNumber);
        public void AddArtists(string cacheKey, int rowNumber);
        public IEnumerable<Artist> GetArtists(string cacheKey, int rowNumber);
    }
}