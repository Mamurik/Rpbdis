using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;

public class EmployeesController : Controller
{
    private readonly RadioStationDbContext _context;

    public EmployeesController(RadioStationDbContext context)
    {
        _context = context;
    }

    // GET: Employees

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees.ToListAsync();
        return View(employees);
    }
}
