using DataLayer.models;

namespace Radiostation.Services.BroadcastSchedulesService
{
    public interface ICachedBroadcastSchedulesService
    {
        public IEnumerable<BroadcastSchedule> GetBroadcastSchedules(int rowNumber);
        public void AddBroadcastSchedules(string cacheKey, int rowNumber);
        public IEnumerable<BroadcastSchedule> GetBroadcastSchedules(string cacheKey, int rowNumber);
    }
}
