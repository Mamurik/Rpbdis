using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;

public class GenresController : Controller
{
    private readonly RadioStationDbContext _context;

    public GenresController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: Genres

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres.ToListAsync();
        return View(genres);
    }
}
