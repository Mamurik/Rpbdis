using System.Collections.Generic;
using RadiostationWeb.Models;

namespace RadiostationWeb.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<BroadcastSchedule> BroadcastSchedules { get; set; } = new List<BroadcastSchedule>();
        public IEnumerable<Record> Records { get; set; } = new List<Record>();
        public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    }
}
