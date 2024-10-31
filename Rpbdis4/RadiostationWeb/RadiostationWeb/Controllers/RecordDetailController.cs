using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;

public class RecordDetailsController : Controller
{
    private readonly RadioStationDbContext _context;

    public RecordDetailsController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: RecordDetails

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var recordDetails = await _context.RecordDetails
            .Include(rd => rd.Record)
            .ToListAsync();
        return View(recordDetails);
    }
}
    