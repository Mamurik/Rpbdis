using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Linq;

public class RecordDetailsController : Controller
{
    private readonly RadioStationDbContext _context;
    private const int PageSize = 10; // ���������� ������� �� ��������

    public RecordDetailsController(RadioStationDbContext context)
    {
        _context = context;
    }

    [Authorize]

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index(int pageNumber = 1, string searchRecord = "", string sortOrder = "asc", string sortBy = "title")
    {
        // ���������� ���������� ���������� � cookie
        if (!string.IsNullOrEmpty(searchRecord))
        {
            Response.Cookies.Append("SearchRecord", searchRecord, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        }
        Response.Cookies.Append("SortOrder", sortOrder, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("SortBy", sortBy, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
        Response.Cookies.Append("PageNumber", pageNumber.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });

        // �������������� ���������� ���������� �� cookie
        searchRecord ??= Request.Cookies["SearchRecord"];
        sortOrder = Request.Cookies["SortOrder"] ?? sortOrder;
        sortBy = Request.Cookies["SortBy"] ?? sortBy;
        pageNumber = int.TryParse(Request.Cookies["PageNumber"], out var pageNum) ? pageNum : pageNumber;

        // ���� searchRecord ������ ��� ����� "���", �� ������������ ���
        if (string.IsNullOrEmpty(searchRecord) || searchRecord.Trim().Equals("���", StringComparison.OrdinalIgnoreCase))
        {
            searchRecord = null; // ����� ����������
        }

        var recordDetailsQuery = _context.RecordDetails.Include(rd => rd.Record).AsQueryable();

        // ���������� ����������
        if (!string.IsNullOrEmpty(searchRecord))
        {
            recordDetailsQuery = recordDetailsQuery.Where(rd => rd.Record.Title.Contains(searchRecord));
        }

        // ����������
        if (sortBy == "date")
        {
            recordDetailsQuery = sortOrder == "desc"
                ? recordDetailsQuery.OrderByDescending(rd => rd.RecordingDate)
                : recordDetailsQuery.OrderBy(rd => rd.RecordingDate);
        }
        else if (sortBy == "duration")
        {
            recordDetailsQuery = sortOrder == "desc"
                ? recordDetailsQuery.OrderByDescending(rd => rd.Duration)
                : recordDetailsQuery.OrderBy(rd => rd.Duration);
        }
        else if (sortBy == "rating")
        {
            recordDetailsQuery = sortOrder == "desc"
                ? recordDetailsQuery.OrderByDescending(rd => rd.Rating)
                : recordDetailsQuery.OrderBy(rd => rd.Rating);
        }
        else // ���������� �� ��������� �� �������� ������
        {
            recordDetailsQuery = sortOrder == "desc"
                ? recordDetailsQuery.OrderByDescending(rd => rd.Record.Title)
                : recordDetailsQuery.OrderBy(rd => rd.Record.Title);
        }

        var totalRecords = await recordDetailsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);
        var recordsToShow = await recordDetailsQuery
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Records = await _context.Records.ToListAsync();
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;
        ViewBag.SearchRecord = searchRecord;
        ViewBag.SortOrder = sortOrder;
        ViewBag.SortBy = sortBy;

        return View(recordsToShow);
    }



    // POST: RecordDetails/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(int recordId, DateOnly recordingDate, TimeOnly duration, int rating)
    {
        var recordDetail = new RecordDetail
        {
            RecordId = recordId,
            RecordingDate = recordingDate,
            Duration = duration,
            Rating = rating
        };

        _context.Add(recordDetail);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: RecordDetails/Edit/{id}
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var recordDetail = await _context.RecordDetails
            .Include(rd => rd.Record)
            .FirstOrDefaultAsync(m => m.RecordDetailId == id);

        if (recordDetail == null)
        {
            return NotFound();
        }

        return Json(new
        {
            recordDetailId = recordDetail.RecordDetailId,
            recordId = recordDetail.RecordId,
            recordingDate = recordDetail.RecordingDate,
            duration = recordDetail.Duration,
            rating = recordDetail.Rating
        });
    }

    // POST: RecordDetails/Edit
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int recordDetailId, int recordId, DateOnly recordingDate, TimeOnly duration, int rating)
    {
        var recordDetail = await _context.RecordDetails.FindAsync(recordDetailId);
        if (recordDetail == null)
        {
            return NotFound();
        }

        recordDetail.RecordId = recordId;
        recordDetail.RecordingDate = recordingDate;
        recordDetail.Duration = duration;
        recordDetail.Rating = rating;

        if (ModelState.IsValid)
        {
            _context.Update(recordDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Records = await _context.Records.ToListAsync();
        return View(nameof(Index), await _context.RecordDetails.Include(rd => rd.Record).ToListAsync());
    }
    // GET: RecordDetails/Delete/{id}
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recordDetail = await _context.RecordDetails
            .Include(rd => rd.Record)
            .FirstOrDefaultAsync(m => m.RecordDetailId == id);

        if (recordDetail == null)
        {
            return NotFound();
        }

        return View(recordDetail);
    }

    // POST: RecordDetails/Delete/{id}
    [Authorize(Roles = "admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recordDetail = await _context.RecordDetails.FindAsync(id);
        if (recordDetail != null)
        {
            _context.RecordDetails.Remove(recordDetail);
            await _context.SaveChangesAsync();
        }

        // Redirect to the index action after deletion
        return RedirectToAction(nameof(Index));
    }

}
