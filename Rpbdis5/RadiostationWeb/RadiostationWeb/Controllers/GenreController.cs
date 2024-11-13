using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Linq;
using System.Threading.Tasks;

public class GenresController : Controller
{
    private readonly RadioStationDbContext _context;
    private const int PageSize = 10; // количество элементов на одной странице

    public GenresController(RadioStationDbContext context)
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
        // Сохранение параметров фильтрации в cookie
        if (!string.IsNullOrEmpty(searchString))
        {
            Response.Cookies.Append("SearchStringGenre", searchString, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }
        Response.Cookies.Append("SortFieldGenre", sortField, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("SortAscGenre", sortAsc.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("PageGenre", page.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });

        // Восстановление параметров фильтрации из cookie
        searchString ??= Request.Cookies["SearchStringGenre"];
        sortField = Request.Cookies["SortFieldGenre"] ?? sortField;
        sortAsc = bool.TryParse(Request.Cookies["SortAscGenre"], out var asc) ? asc : sortAsc;
        page = int.TryParse(Request.Cookies["PageGenre"], out var pageNum) ? pageNum : page;

        // Если searchString пустое или равно "Все", то игнорировать его
        if (string.IsNullOrEmpty(searchString) || searchString.Trim().Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            searchString = null; // Сброс фильтрации
        }

        var genresQuery = _context.Genres.AsQueryable();

        // Фильтрация по имени жанра
        if (!string.IsNullOrEmpty(searchString))
        {
            genresQuery = genresQuery.Where(g => g.Name.Contains(searchString));
        }

        // Сортировка
        genresQuery = sortField switch
        {
            "Name" => sortAsc ? genresQuery.OrderBy(g => g.Name) : genresQuery.OrderByDescending(g => g.Name),
            "Description" => sortAsc ? genresQuery.OrderBy(g => g.Description) : genresQuery.OrderByDescending(g => g.Description),
            _ => genresQuery
        };

        var totalItems = await genresQuery.CountAsync();
        var pagedGenres = await genresQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.TotalRecords = totalItems;
        ViewBag.PageSize = PageSize;
        ViewBag.CurrentPage = page;
        ViewBag.SearchString = searchString; // сохраняем строку поиска
        ViewBag.SortField = sortField;
        ViewBag.SortAsc = sortAsc;

        return View(pagedGenres);
    }

    [Authorize(Roles = "admin")]
    // GET: Genres/Create
    public IActionResult Create()
    {
        return View();
    }
    [Authorize(Roles = "admin")]
    // POST: Genres/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Genre genre)
    {
        if (ModelState.IsValid)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(genre);
    }
    [Authorize(Roles = "admin")]
    // GET: Genres/Edit/{id} - Возвращает данные жанра в формате JSON
    public async Task<IActionResult> Edit(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        return Json(new
        {
            genreId = genre.GenreId,
            name = genre.Name,
            description = genre.Description
        });
    }

    // POST: Genres/Edit - Сохраняет изменения жанра
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int genreId, string name, string description)
    {
        var genre = await _context.Genres.FindAsync(genreId);
        if (genre == null)
        {
            return NotFound();
        }

        genre.Name = name;
        genre.Description = description;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(genre.GenreId))
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
        return View(genre);
    }
    [Authorize(Roles = "admin")]
    // GET: Genres/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(m => m.GenreId == id);
        if (genre == null)
        {
            return NotFound();
        }
        return View(genre);
    }
    [Authorize(Roles = "admin")]
    // POST: Genres/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre != null)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index)); // Перенаправление на Index
    }



    private bool GenreExists(int id)
    {
        return _context.Genres.Any(e => e.GenreId == id);
    }
}
