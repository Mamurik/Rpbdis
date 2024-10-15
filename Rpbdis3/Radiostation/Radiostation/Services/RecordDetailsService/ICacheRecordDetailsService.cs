using DataLayer.models;

namespace Radiostation.Services.RecordDetailsService
{
    public interface ICacheRecordDetailsService
    {
        IEnumerable<RecordDetail> GetRecordDetails(int rowNumber);
        void AddRecordDetails(string cacheKey, int rowNumber);
        IEnumerable<RecordDetail> GetRecordDetails(string cacheKey, int rowNumber);
    }
}
