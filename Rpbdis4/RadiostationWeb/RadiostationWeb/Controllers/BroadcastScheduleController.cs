using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;

public class BroadcastSchedulesController : Controller
{
    private readonly RadioStationDbContext _context;

    public BroadcastSchedulesController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: BroadcastSchedules

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var schedules = await _context.BroadcastSchedules
            .Include(bs => bs.Employee) // Подгрузка работника
            .Include(bs => bs.Record)    // Подгрузка записи
            .ToListAsync();
        return View(schedules);
    }

}
