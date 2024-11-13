using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Linq;
using System.Threading.Tasks;

public class ArtistsController : Controller
{
    private readonly RadioStationDbContext _context;
    private const int PageSize = 10; // количество элементов на одной странице

    public ArtistsController(RadioStationDbContext context)
    {
        _context = context;
    }
    [Authorize]

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index(
     string searchString,
     string sortField = "Name",
     bool sortAsc = true,
     int page = 1)
    {
        // Сохранение параметров фильтрации в cookie для артистов
        if (!string.IsNullOrEmpty(searchString))
        {
            Response.Cookies.Append("SearchStringArtist", searchString, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }
        Response.Cookies.Append("SortField", sortField, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("SortAsc", sortAsc.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("Page", page.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });

        // Восстановление параметров фильтрации из cookie для артистов
        searchString ??= Request.Cookies["SearchStringArtist"];
        sortField = Request.Cookies["SortField"] ?? sortField;
        sortAsc = bool.TryParse(Request.Cookies["SortAsc"], out var asc) ? asc : sortAsc;
        page = int.TryParse(Request.Cookies["Page"], out var pageNum) ? pageNum : page;

        // Если searchString пустое или равно "Все", то игнорировать его
        if (string.IsNullOrEmpty(searchString) || searchString.Trim().Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            searchString = null; // Сброс фильтрации
        }

        var artistsQuery = _context.Artists.AsQueryable();

        // Фильтрация по имени артиста
        if (!string.IsNullOrEmpty(searchString))
        {
            artistsQuery = artistsQuery.Where(a => a.Name.Contains(searchString));
        }

        // Сортировка
        artistsQuery = sortField switch
        {
            "Name" => sortAsc ? artistsQuery.OrderBy(a => a.Name) : artistsQuery.OrderByDescending(a => a.Name),
            "Members" => sortAsc ? artistsQuery.OrderBy(a => a.Members) : artistsQuery.OrderByDescending(a => a.Members),
            "Description" => sortAsc ? artistsQuery.OrderBy(a => a.Description) : artistsQuery.OrderByDescending(a => a.Description),
            _ => artistsQuery
        };

        var totalItems = await artistsQuery.CountAsync();
        var pagedArtists = await artistsQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.TotalRecords = totalItems;
        ViewBag.PageSize = PageSize;
        ViewBag.CurrentPage = page;
        ViewBag.SearchString = searchString;
        ViewBag.SortField = sortField;
        ViewBag.SortAsc = sortAsc;

        return View(pagedArtists);
    }






    [Authorize(Roles = "admin")]
    // GET: Artists/Create
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    // POST: Artists/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Members,Description")] Artist artist)
    {
        if (ModelState.IsValid)
        {
            _context.Add(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(artist);
    }

    [Authorize(Roles = "admin")]
    // GET: Artists/Edit/{id} - Возвращает данные артиста в формате JSON
    public async Task<IActionResult> Edit(int id)
    {
        var artist = await _context.Artists.FindAsync(id);
        if (artist == null)
        {
            return NotFound();
        }

        return Json(new
        {
            artistId = artist.ArtistId,
            name = artist.Name,
            members = artist.Members,
            description = artist.Description
        });
    }

    [Authorize(Roles = "admin")]
    // POST: Artists/Edit - Сохраняет изменения артиста
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int artistId, string name, string members, string description)
    {
        var artist = await _context.Artists.FindAsync(artistId);
        if (artist == null)
        {
            return NotFound();
        }

        artist.Name = name;
        artist.Members = members;
        artist.Description = description;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(artist);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(artist.ArtistId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(artist);
    }


    [Authorize(Roles = "admin")]
    // GET: Artists/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var artist = await _context.Artists.FirstOrDefaultAsync(m => m.ArtistId == id);
        if (artist == null)
        {
            return NotFound();
        }

        return View(artist);
    }

    // POST: Artists/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var artist = await _context.Artists.FindAsync(id);
        if (artist != null)
        {
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ArtistExists(int id)
    {
        return _context.Artists.Any(e => e.ArtistId == id);
    }
}
