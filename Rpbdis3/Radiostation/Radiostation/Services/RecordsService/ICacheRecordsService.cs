using System.Collections.Generic;
using DataLayer.models;

namespace Radiostation.Services.RecordsService
{
    public interface ICacheRecordsService
    {
        IEnumerable<Record> GetRecords(int rowNumber);
        void AddRecords(string cacheKey, int rowNumber);
        IEnumerable<Record> GetRecords(string cacheKey, int rowNumber);
    }
}
