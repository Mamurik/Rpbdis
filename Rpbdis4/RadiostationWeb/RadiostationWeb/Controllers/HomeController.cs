using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using RadiostationWeb.ViewModels; // ���������, ��� ��� ���������
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
                .Include(bs => bs.Employee) // ���������� ���������
                .Include(bs => bs.Record)    // ���������� ������
                .Take(20) // �������� ������ 20 ����������
                .ToListAsync();

            var records = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Genre)
                .Take(20) // �������� ������ 20 �������
                .ToListAsync();

            var employees = await _context.Employees
                .Take(20) // �������� ������ 20 ����������
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
