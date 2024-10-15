using System.Collections.Generic;
using DataLayer.models;

namespace Radiostation.Services.EmployeesService
{
    public interface ICacheEmployeesService
    {
        IEnumerable<Employee> GetEmployees(int rowNumber);
        void AddEmployees(string cacheKey, int rowNumber);
        IEnumerable<Employee> GetEmployees(string cacheKey, int rowNumber);
    }
}
