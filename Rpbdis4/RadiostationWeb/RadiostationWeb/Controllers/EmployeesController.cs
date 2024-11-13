    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using RadiostationWeb.Data;
    using RadiostationWeb.Models;
    using System.Drawing.Printing;

    public class EmployeesController : Controller
    {
        private readonly RadioStationDbContext _context;
        private const int PageSize = 10;
        public EmployeesController(RadioStationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
        public async Task<IActionResult> Index(
         string searchString,
         string educationFilter,
         string sortField = "FullName",
         bool sortAsc = true,
         int page = 1)
        {
            // Получение значений фильтров из cookies, если они не переданы
            if (string.IsNullOrEmpty(searchString))
            {
                searchString = Request.Cookies["SearchString"] ?? "";
            }
            else
            {
                // Сохранение значений фильтров в cookies
                Response.Cookies.Append("SearchString", searchString, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
            }

            if (string.IsNullOrEmpty(educationFilter))
            {
                educationFilter = Request.Cookies["EducationFilter"] ?? "";
            }
            else
            {
                // Сохранение значений фильтров в cookies
                Response.Cookies.Append("EducationFilter", educationFilter, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
            }

            var employeesQuery = _context.Employees.AsQueryable();

            // Фильтрация по имени работника (игнорирование фильтра при значении "все")
            if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("все", StringComparison.OrdinalIgnoreCase))
            {
                employeesQuery = employeesQuery.Where(e => e.FullName.Contains(searchString));
            }
            else if (searchString.Equals("все", StringComparison.OrdinalIgnoreCase))
            {
                // Очистка значения для отображения в инпуте
                searchString = string.Empty;
            }

            // Фильтрация по образованию (игнорирование фильтра при значении "все")
            if (!string.IsNullOrEmpty(educationFilter) && !educationFilter.Equals("все", StringComparison.OrdinalIgnoreCase))
            {
                employeesQuery = employeesQuery.Where(e => e.Education.Contains(educationFilter));
            }
            else if (educationFilter.Equals("все", StringComparison.OrdinalIgnoreCase))
            {
                // Очистка значения для отображения в инпуте
                educationFilter = string.Empty;
            }

            // Сортировка
            employeesQuery = sortField switch
            {
                "FullName" => sortAsc ? employeesQuery.OrderBy(e => e.FullName) : employeesQuery.OrderByDescending(e => e.FullName),
                "Education" => sortAsc ? employeesQuery.OrderBy(e => e.Education) : employeesQuery.OrderByDescending(e => e.Education),
                "Position" => sortAsc ? employeesQuery.OrderBy(e => e.Position) : employeesQuery.OrderByDescending(e => e.Position),
                _ => employeesQuery
            };

            var totalItems = await employeesQuery.CountAsync();
            var pagedEmployees = await employeesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.TotalRecords = totalItems;
            ViewBag.PageSize = PageSize;
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;
            ViewBag.EducationFilter = educationFilter;
            ViewBag.SortField = sortField;
            ViewBag.SortAsc = sortAsc;

            return View(pagedEmployees);
        }




        [Authorize(Roles = "admin")]
        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Education,Position")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        [Authorize(Roles = "admin")]
        // GET: Employees/Edit/{id} - Возвращает данные сотрудника в формате JSON
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Json(new
            {
                employeeId = employee.EmployeeId,
                fullName = employee.FullName,
                education = employee.Education,
                position = employee.Position
            });
        }
        [Authorize(Roles = "admin")]
        // POST: Employees/Edit - Сохраняет изменения сотрудника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int employeeId, string fullName, string education, string position)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            employee.FullName = fullName;
            employee.Education = education;
            employee.Position = position;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            return View(employee);
        }
        [Authorize(Roles = "admin")]
        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [Authorize(Roles = "admin")]
        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
