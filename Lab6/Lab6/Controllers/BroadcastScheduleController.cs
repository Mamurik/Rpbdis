using Lab6.Data;
using Lab6.Models;
using Lab6.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Controllers
{
    [Route("api/[controller]")]
    public class BroadcastScheduleController(RadioStationDbContext context) : Controller
    {
        private readonly RadioStationDbContext _context = context;

        /// <summary>
        /// Получение списка расписания
        /// </summary>
        // GET api/values
        [HttpGet]
        [Produces("application/json")]
        public List<BroadcastScheduleViewModel> Get()
        {
            IQueryable<BroadcastScheduleViewModel> svm = _context.BroadcastSchedules
                .Include(bs => bs.Employee)
                .Include(bs => bs.Record)
                .Select(bs => new BroadcastScheduleViewModel
                {
                    ScheduleId = bs.ScheduleId,
                    BroadcastDate = bs.BroadcastDate,
                    EmployeeId = bs.EmployeeId,
                    RecordId = bs.RecordId,
                    EmployeeName = bs.Employee.FullName,
                    RecordTitle = bs.Record.Title
                });
            return svm.Take(500).ToList();
        }


        /// <summary>
        /// Получение списка расписаний, удовлетворяющих заданному условию
        /// </summary>
        /// <remarks>
        /// Описание параметров
        /// </remarks>
        /// <param name="employeeId">Код сотрудника</param>
        /// <param name="recordId">Код артиста</param>
        /// <returns>JSON</returns>
        [HttpGet("FilteredSchedules")]
        [Produces("application/json")]
        public List<BroadcastScheduleViewModel> GetFilteredBroadcasts(int employeeId, int recordId)
        {
            IQueryable<BroadcastScheduleViewModel> svm = _context.BroadcastSchedules
                .Include(bs => bs.Employee)
                .Include(bs => bs.Record)
                .Select(bs => new BroadcastScheduleViewModel
                {
                    ScheduleId = bs.ScheduleId,
                    BroadcastDate = bs.BroadcastDate,
                    EmployeeId = bs.EmployeeId,
                    RecordId = bs.RecordId,
                    EmployeeName = bs.Employee.FullName,
                    RecordTitle = bs.Record.Title
                });

            if (employeeId > 0)
            {
                svm = svm.Where(op => op.EmployeeId == employeeId);
            }
            if (recordId > 0)
            {
                svm = svm.Where(op => op.RecordId == recordId);
            }

            return svm.Take(500).OrderBy(o => o.ScheduleId).ToList();
        }


        /// <summary>
        /// Список Запсией
        /// </summary>
        [HttpGet("records")]
        [Produces("application/json")]
        public IEnumerable<Record> GetRecords()
        {
            return _context.Records.ToList();
        }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        [HttpGet("employees")]
        [Produces("application/json")]
        public IEnumerable<Employee> GetEmployees()
        {
            return _context.Employees.ToList();
        }

        /// <summary>
        /// Получение данных одного расписания
        /// </summary>
        /// <remarks>
        /// Описание параметра
        /// </remarks>
        /// <param name="id">Код расписания</param>
        /// <returns>JSON</returns>
        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            BroadcastSchedule broadcastSchedule = _context.BroadcastSchedules.FirstOrDefault(x => x.ScheduleId == id);
            if (broadcastSchedule == null)
                return NotFound();
            return new ObjectResult(broadcastSchedule);
        }

        /// <summary>
        /// Регистрация нового расписания
        /// </summary>
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] BroadcastSchedule broadcastSchedule)
        {
            if (broadcastSchedule == null)
            {
                return BadRequest();
            }
            _context.BroadcastSchedules.Add(broadcastSchedule);
            _context.SaveChanges();
            return Ok(broadcastSchedule);
        }

        /// <summary>
        /// Обновление данных одного расписания
        /// </summary>
        /// <remarks>
        /// Объект передается в теле запроса
        /// </remarks>
        /// <param name="broadcastSchedule">объект, определяющий расписания</param>
        /// <returns>Статус</returns>
        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] BroadcastSchedule broadcastSchedule)
        {
            if (broadcastSchedule == null)
            {
                return BadRequest();
            }
            if (!_context.BroadcastSchedules.Any(x => x.ScheduleId == broadcastSchedule.ScheduleId))
            {
                return NotFound();
            }
            _context.Update(broadcastSchedule);
            _context.SaveChanges();
            return Ok(broadcastSchedule);
        }

        /// <summary>
        /// Удаление данных одной расписания
        /// </summary>
        /// <remarks>
        /// Описание параметра
        /// </remarks>
        /// <param name="id">Код расписания</param>
        /// <returns>Статус</returns>
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            BroadcastSchedule broadcastSchedule = _context.BroadcastSchedules.FirstOrDefault(x => x.ScheduleId == id);
            if (broadcastSchedule == null)
            {
                return NotFound();
            }
            _context.BroadcastSchedules.Remove(broadcastSchedule);
            _context.SaveChanges();
            return Ok(broadcastSchedule);
        }
    }
}