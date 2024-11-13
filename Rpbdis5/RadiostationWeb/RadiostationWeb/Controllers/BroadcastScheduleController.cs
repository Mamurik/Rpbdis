using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

public class BroadcastSchedulesController : Controller
{
    private readonly RadioStationDbContext _context;

    public BroadcastSchedulesController(RadioStationDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index(
      string searchString,
      DateTime? startDate,
      DateTime? endDate,
      string sortField = "Employee.FullName",
      bool sortAsc = true,
      int page = 1,
      int pageSize = 10,
      bool? currentWeek = null, // Параметр для отображения расписания текущей недели
      string recordName = null) // Новый параметр для фильтрации по названию записи
    {
        // Сохраняем параметры фильтрации в cookies
        if (!string.IsNullOrEmpty(searchString))
        {
            Response.Cookies.Append("SearchStringSchedule", searchString, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }

        if (startDate.HasValue)
        {
            Response.Cookies.Append("StartDate", startDate.Value.ToString("o"), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }

        if (endDate.HasValue)
        {
            Response.Cookies.Append("EndDate", endDate.Value.ToString("o"), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }

        if (!string.IsNullOrEmpty(recordName))
        {
            Response.Cookies.Append("RecordName", recordName, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }

        Response.Cookies.Append("SortField", sortField, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("SortAsc", sortAsc.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("Page", page.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });

        // Восстановление параметров фильтрации из cookies
        searchString ??= Request.Cookies["SearchStringSchedule"];
        recordName ??= Request.Cookies["RecordName"];
        sortField = Request.Cookies["SortField"] ?? sortField;
        sortAsc = bool.TryParse(Request.Cookies["SortAsc"], out var asc) ? asc : sortAsc;
        page = int.TryParse(Request.Cookies["Page"], out var pageNum) ? pageNum : page;

        // Если searchString пустое или равно "Все", сбросить фильтрацию
        if (string.IsNullOrEmpty(searchString) || searchString.Trim().Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            searchString = null; // Сбросить фильтрацию
        }
        // Если searchString пустое или равно "Все", сбросить фильтрацию
        if (string.IsNullOrEmpty(recordName) || recordName.Trim().Equals("Все", StringComparison.OrdinalIgnoreCase))
        {
            recordName = null; // Сбросить фильтрацию
        }

        var schedulesQuery = _context.BroadcastSchedules
            .Include(bs => bs.Employee)
            .Include(bs => bs.Record)
            .AsQueryable();

        // Фильтрация по имени сотрудника
        if (!string.IsNullOrEmpty(searchString))
        {
            schedulesQuery = schedulesQuery.Where(bs => bs.Employee.FullName.Contains(searchString));
        }

        // Фильтрация по названию записи
        if (!string.IsNullOrEmpty(recordName))
        {
            schedulesQuery = schedulesQuery.Where(bs => bs.Record.Title.Contains(recordName));
        }

        // Сортировка расписаний
        schedulesQuery = sortField switch
        {
            "Employee.FullName" => sortAsc ? schedulesQuery.OrderBy(bs => bs.Employee.FullName) : schedulesQuery.OrderByDescending(bs => bs.Employee.FullName),
            "BroadcastDate" => sortAsc ? schedulesQuery.OrderBy(bs => bs.BroadcastDate) : schedulesQuery.OrderByDescending(bs => bs.BroadcastDate),
            _ => schedulesQuery
        };

        // Фильтрация по диапазону времени
        if (startDate.HasValue)
        {
            schedulesQuery = schedulesQuery.Where(bs => bs.BroadcastDate >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            schedulesQuery = schedulesQuery.Where(bs => bs.BroadcastDate <= endDate.Value);
        }

        // Фильтрация по текущей неделе
        if (currentWeek.HasValue && currentWeek.Value)
        {
            var currentDate = DateTime.Now;
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);  // Воскресенье
            var endOfWeek = startOfWeek.AddDays(6);  // Суббота
            schedulesQuery = schedulesQuery.Where(bs => bs.BroadcastDate >= startOfWeek && bs.BroadcastDate <= endOfWeek);
        }

        var totalRecords = await schedulesQuery.CountAsync();
        var pagedSchedules = await schedulesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalRecords = totalRecords;
        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = page;
        ViewBag.SearchString = searchString;
        ViewBag.SortField = sortField;
        ViewBag.SortAsc = sortAsc;
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;
        ViewBag.CurrentWeek = currentWeek;
        ViewBag.RecordName = recordName; // Передаем значение фильтра в ViewBag

        ViewBag.Employees = await _context.Employees.ToListAsync();
        ViewBag.Records = await _context.Records.ToListAsync();

        return View(pagedSchedules);
    }



    [Authorize(Roles = "admin")]
    // POST: BroadcastSchedules/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int employeeId, int recordId, DateTime BroadcastDate)
    {
        var schedule = new BroadcastSchedule
        {
            EmployeeId = employeeId,
            RecordId = recordId,
            BroadcastDate = BroadcastDate
        };

        _context.Add(schedule);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "admin")]
    // GET: BroadcastSchedules/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _context.BroadcastSchedules
            .Include(bs => bs.Employee)
            .Include(bs => bs.Record)
            .FirstOrDefaultAsync(m => m.ScheduleId == id);

        if (schedule == null)
        {
            return NotFound();
        }

        return Json(new
        {
            scheduleId = schedule.ScheduleId,
            employeeId = schedule.EmployeeId,
            recordId = schedule.RecordId,
            BroadcastDate = schedule.BroadcastDate
        });
    }
    [Authorize(Roles = "admin")]
    // POST: BroadcastSchedules/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int scheduleId, int employeeId, int recordId, DateTime BroadcastDate)
    {
        var schedule = await _context.BroadcastSchedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            return NotFound();
        }

        schedule.EmployeeId = employeeId;
        schedule.RecordId = recordId;
        schedule.BroadcastDate = BroadcastDate;

        if (ModelState.IsValid)
        {
            _context.Update(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Employees = await _context.Employees.ToListAsync();
        ViewBag.Records = await _context.Records.ToListAsync();

        return View(nameof(Index), await _context.BroadcastSchedules.Include(bs => bs.Employee).Include(bs => bs.Record).ToListAsync());
    }
    [Authorize(Roles = "admin")]
    // GET: BroadcastSchedules/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var schedule = await _context.BroadcastSchedules
            .Include(bs => bs.Employee)
            .Include(bs => bs.Record)
            .FirstOrDefaultAsync(m => m.ScheduleId == id);
        if (schedule == null)
        {
            return NotFound();
        }

        return View(schedule);
    }
    [Authorize(Roles = "admin")]
    // POST: BroadcastSchedules/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var schedule = await _context.BroadcastSchedules.FindAsync(id);
        if (schedule != null)
        {
            _context.BroadcastSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
