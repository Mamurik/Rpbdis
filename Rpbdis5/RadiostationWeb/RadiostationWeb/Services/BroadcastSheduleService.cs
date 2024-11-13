using RadiostationWeb.ViewModels;

using RadiostationWeb.Data;
using RadiostationWeb.Models;

namespace RadiostationWeb.Services
{
    // Класс выборки 10 записей из всех таблиц 
    public class BroadcastSheduleService(RadioStationDbContext context) : IBroadcastSheduleService
    {
        private readonly RadioStationDbContext _context = context;

        public HomeViewModel GetHomeViewModel(int numberRows = 10)
        {
            var employees = _context.Employees.Take(numberRows).ToList();
            var records = _context.Records.Take(numberRows).ToList();
            List<BroadcastSchedule> broadcastSchedules = [.. _context.BroadcastSchedules
                .OrderByDescending(d => d.BroadcastDate)
                .Select(s => new BroadcastSchedule
                {
                    ScheduleId = s.ScheduleId,
                    EmployeeId = s.EmployeeId,
                    RecordId = s.RecordId,
                    BroadcastDate = s.BroadcastDate,
                })
                .Take(numberRows)];

            HomeViewModel homeViewModel = new()
            {
                BroadcastSchedules = broadcastSchedules,
                Records = records,
                Employees = employees
            };
            return homeViewModel;
        }
    }
}
