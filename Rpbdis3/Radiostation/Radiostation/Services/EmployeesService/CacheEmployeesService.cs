using Microsoft.Extensions.Caching.Memory;
using DataLayer.Data;
using DataLayer.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Radiostation.Services.EmployeesService
{
    public class CacheEmployeesService : ICacheEmployeesService
    {
        private readonly IMemoryCache _cache;
        private readonly RadioStationDbContext _context;

        public CacheEmployeesService(IMemoryCache cache, RadioStationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public void AddEmployees(string cacheKey, int rowNumber)
        {
            IEnumerable<Employee> employees = _context.Employees.Take(rowNumber).ToList();
            if (employees != null)
            {
                _cache.Set(cacheKey, employees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(292)
                });
            }
        }

        public IEnumerable<Employee> GetEmployees(int rowNumber)
        {
            return _context.Employees
                           .Take(rowNumber)
                           .ToList();
        }

        public IEnumerable<Employee> GetEmployees(string cacheKey, int rowNumber)
        {
            IEnumerable<Employee> employees;
            if (!_cache.TryGetValue(cacheKey, out employees))
            {
                employees = _context.Employees.Take(rowNumber).ToList();
                if (employees != null)
                {
                    _cache.Set(cacheKey, employees, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(292)));
                }
            }
            return employees;
        }
    }
}
