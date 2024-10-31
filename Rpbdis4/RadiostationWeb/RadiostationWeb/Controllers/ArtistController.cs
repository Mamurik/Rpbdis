using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;

public class ArtistsController : Controller
{
    private readonly RadioStationDbContext _context;

    public ArtistsController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: Artists
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var artists = await _context.Artists.ToListAsync();
        return View(artists);
    }
}
