using DataLayer.models;

namespace Radiostation.Services.GenresService
{
    public interface ICacheGenresService
    {
        IEnumerable<Genre> GetGenres(int rowNumber);
        void AddGenres(string cacheKey, int rowNumber);
        IEnumerable<Genre> GetGenres(string cacheKey, int rowNumber);
    }
}
