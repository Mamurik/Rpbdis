using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using RadiostationWeb.ViewModels;

public class RecordsController : Controller
{
    private readonly RadioStationDbContext _context;

    public RecordsController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: Records
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var broadcastSchedules = await _context.BroadcastSchedules.ToListAsync();
        var records = await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Genre)
            .ToListAsync();
        var employees = await _context.Employees.ToListAsync();

        var viewModel = new HomeViewModel
        {
            BroadcastSchedules = broadcastSchedules,
            Records = records,
            Employees = employees
        };

        return View(viewModel);
    }

}
