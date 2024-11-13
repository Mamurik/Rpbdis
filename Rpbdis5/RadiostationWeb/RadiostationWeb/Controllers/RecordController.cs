using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using Microsoft.AspNetCore.Authorization;

public class RecordsController : Controller
{
    private readonly RadioStationDbContext _context;

    public RecordsController(RadioStationDbContext context)
    {
        _context = context;
    }
    [Authorize]
    // GET: Records
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index(string artistFilter, string genreFilter, string sortColumn, string sortDirection, int page = 1, int pageSize = 10)
    {
        // Сохранение параметров фильтрации в cookie
        if (!string.IsNullOrEmpty(artistFilter))
        {
            Response.Cookies.Append("ArtistFilter", artistFilter, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }
        if (!string.IsNullOrEmpty(genreFilter))
        {
            Response.Cookies.Append("GenreFilter", genreFilter, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }
        Response.Cookies.Append("SortColumn", sortColumn ?? "Title", new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("SortDirection", sortDirection ?? "asc", new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("Page", page.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });

        // Восстановление параметров фильтрации из cookie
        artistFilter ??= Request.Cookies["ArtistFilter"];
        genreFilter ??= Request.Cookies["GenreFilter"];
        sortColumn = Request.Cookies["SortColumn"] ?? sortColumn;
        sortDirection = Request.Cookies["SortDirection"] ?? sortDirection;
        page = int.TryParse(Request.Cookies["Page"], out var pageNum) ? pageNum : page;

        // Если фильтры пустые или равно "Все", сбрасываем фильтрацию
        if (string.IsNullOrEmpty(artistFilter) || artistFilter.Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            artistFilter = null;
        }
        if (string.IsNullOrEmpty(genreFilter) || genreFilter.Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            genreFilter = null;
        }

        var recordsQuery = _context.Records.Include(r => r.Artist).Include(r => r.Genre).AsQueryable();

        // Фильтрация по артисту
        if (!string.IsNullOrEmpty(artistFilter))
        {
            recordsQuery = recordsQuery.Where(r => r.Artist.Name.Contains(artistFilter));
        }

        // Фильтрация по жанру
        if (!string.IsNullOrEmpty(genreFilter))
        {
            recordsQuery = recordsQuery.Where(r => r.Genre.Name.Contains(genreFilter));
        }

        // Сортировка
        recordsQuery = sortColumn switch
        {
            "Title" => sortDirection == "desc" ? recordsQuery.OrderByDescending(r => r.Title) : recordsQuery.OrderBy(r => r.Title),
            "Artist" => sortDirection == "desc" ? recordsQuery.OrderByDescending(r => r.Artist.Name) : recordsQuery.OrderBy(r => r.Artist.Name),
            "Album" => sortDirection == "desc" ? recordsQuery.OrderByDescending(r => r.Album) : recordsQuery.OrderBy(r => r.Album),
            "Year" => sortDirection == "desc" ? recordsQuery.OrderByDescending(r => r.Year) : recordsQuery.OrderBy(r => r.Year),
            "Genre" => sortDirection == "desc" ? recordsQuery.OrderByDescending(r => r.Genre.Name) : recordsQuery.OrderBy(r => r.Genre.Name),
            _ => recordsQuery.OrderBy(r => r.Title)
        };

        var totalRecords = await recordsQuery.CountAsync();
        var pagedRecords = await recordsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalRecords = totalRecords;
        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = page;
        ViewBag.ArtistFilter = artistFilter;
        ViewBag.GenreFilter = genreFilter;
        ViewBag.SortColumn = sortColumn;
        ViewBag.SortDirection = sortDirection == "asc" ? "desc" : "asc";

        ViewBag.Artists = await _context.Artists.ToListAsync();
        ViewBag.Genres = await _context.Genres.ToListAsync();

        return View(pagedRecords);
    }
    [Authorize]
    // GET: Records/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Получение записи вместе с деталями записи (если они есть)
        var record = await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Genre)
            .Include(r => r.RecordDetail) // Включаем RecordDetail
            .FirstOrDefaultAsync(r => r.RecordId == id);

        if (record == null)
        {
            return NotFound();
        }

        return View(record);
    }

    [Authorize(Roles = "admin")]
    // POST: Records/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string album, int year, int artistId, int genreId)
    {
        var record = new Record
        {
            Title = title,
            Album = album,
            Year = year,
            ArtistId = artistId,
            GenreId = genreId
        };

        _context.Add(record);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "admin")]
    // GET: Records/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var record = await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Genre)
            .FirstOrDefaultAsync(m => m.RecordId == id);

        if (record == null)
        {
            return NotFound();
        }

        return Json(new
        {
            recordId = record.RecordId,
            title = record.Title,
            album = record.Album,
            year = record.Year,
            artistId = record.ArtistId,
            genreId = record.GenreId
        });
    }
    [Authorize(Roles = "admin")]
    // POST: Records/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int recordId, string title, string album, int year, int artistId, int genreId)
    {
        var record = await _context.Records.FindAsync(recordId);
        if (record == null)
        {
            return NotFound();
        }

        record.Title = title;
        record.Album = album;
        record.Year = year;
        record.ArtistId = artistId;
        record.GenreId = genreId;

        if (ModelState.IsValid)
        {
            _context.Update(record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Artists = await _context.Artists.ToListAsync();
        ViewBag.Genres = await _context.Genres.ToListAsync();

        return View(nameof(Index), await _context.Records.Include(r => r.Artist).Include(r => r.Genre).ToListAsync());
    }
    [Authorize(Roles = "admin")]
    // GET: Records/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var record = await _context.Records
            .Include(r => r.Artist)
            .Include(r => r.Genre)
            .FirstOrDefaultAsync(m => m.RecordId == id);
        if (record == null)
        {
            return NotFound();
        }

        return View(record);
    }
    [Authorize(Roles = "admin")]
    // POST: Records/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await _context.Records.FindAsync(id);
        if (record != null)
        {
            _context.Records.Remove(record);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool RecordExists(int id)
    {
        return _context.Records.Any(e => e.RecordId == id);
    }
}
