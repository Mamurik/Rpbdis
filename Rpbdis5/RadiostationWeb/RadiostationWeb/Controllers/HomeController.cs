using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using RadiostationWeb.ViewModels; // Убедитесь, что это добавлено
using System.Linq;

namespace RadiostationWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly RadioStationDbContext _context;

        public HomeController(RadioStationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var broadcastSchedules = await _context.BroadcastSchedules
                .Include(bs => bs.Employee) // Подгружаем работника
                .Include(bs => bs.Record)    // Подгружаем запись
                .Take(20) // Получаем первые 20 расписаний
                .ToListAsync();

            var records = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Genre)
                .Take(20) // Получаем первые 20 записей
                .ToListAsync();

            var employees = await _context.Employees
                .Take(20) // Получаем первых 20 работников
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                BroadcastSchedules = broadcastSchedules,
                Records = records,
                Employees = employees
            };

            return View(viewModel);
        }

    }
}
