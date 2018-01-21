using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;
using ScheduleApp.Web.Models.API;

namespace ScheduleApp.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Shift")]
    [AllowAnonymous]
    public class ShiftController : Controller
    {
        private readonly ScheduleContext _context;

        public ShiftController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: api/Shift/Active
        [HttpGet("Active")]
        public IEnumerable<DateEntry> GetShiftsOfActiveMonth()
        {
            DateTime today = DateTime.Now;

            var shiftEntries = (from shifts in _context.Shift.AsQueryable()
                                join schedules in _context.Schedule.AsQueryable()
                                on shifts.Id equals schedules.ShiftId
                                where shifts.ShiftDate != null && shifts.ShiftDate.Value.Month == today.Month
                                select new DateEntry
                                {
                                    Date = (DateTime)shifts.ShiftDate,
                                    DayType = "WORKDAY",
                                    UserId = (int)schedules.UserId,
                                    Username = schedules.User.Username,
                                    StartHour = 8,
                                    EndHour = 16,
                                    ShiftId = shifts.Id
                                }).ToList();

            return shiftEntries;
        }

        // GET: api/Shift/All
        [HttpGet("All")]
        public IEnumerable<DateEntry> GetAllShifts()
        {
            DateTime today = DateTime.Now;

            var shiftEntries = (from shifts in _context.Shift.AsQueryable()
                                join schedules in _context.Schedule.AsQueryable()
                                on shifts.Id equals schedules.ShiftId
                                where shifts.ShiftDate != null
                                select new DateEntry
                                {
                                    Date = (DateTime)shifts.ShiftDate,
                                    DayType = "WORKDAY",
                                    UserId = (int)schedules.UserId,
                                    Username = schedules.User.Username,
                                    StartHour = 8,
                                    EndHour = 16,
                                    ShiftId = shifts.Id
                                }).ToList();

            return shiftEntries;
        }

        // GET: api/Shift/Selection
        [HttpGet("Selection")]
        public IEnumerable<Shift> Selection([FromRoute] DateTime intervalStartDate, [FromRoute] DateTime intervalEndDate)
        {
            return _context.Shift.Where(s => s.ShiftDate >= intervalStartDate && s.ShiftDate <= intervalEndDate);
        }

        // GET: api/Shift/User/5
        [HttpGet("User/{id}")]
        public IEnumerable<DateEntry> GetUserShifts([FromRoute] int id)
        {
            return _context.Schedule.Where(t => t.UserId == id).Select(s => new DateEntry
            {
                Date = (DateTime)s.Shift.ShiftDate,
                DayType = "WORKDAY",
                UserId = (int)s.UserId,
                Username = s.User.Username,
                StartHour = 8,
                EndHour = 16,
                ShiftId = (int)s.ShiftId
            });
        }

        // GET: Api/Shift/Today
        [HttpGet("Today")]
        public TodayEntry Today()
        {
            var dateEntry = (from shifts in _context.Shift.AsQueryable()
                    join schedules in _context.Schedule.AsQueryable()
                    on shifts.Id equals schedules.ShiftId
                    where shifts.ShiftDate.Value.Year == DateTime.Today.Year
                    && shifts.ShiftDate.Value.Month == DateTime.Today.Month
                    && shifts.ShiftDate.Value.Day == DateTime.Today.Day
                    select new DateEntry
                    {
                        Date = (DateTime)shifts.ShiftDate,
                        DayType = "WORKDAY",
                        UserId = (int)schedules.UserId,
                        Username = schedules.User.Username,
                        StartHour = 8,
                        EndHour = 16,
                        ShiftId = shifts.Id
                    }).SingleOrDefault();
            var todayEntry = new TodayEntry();
            todayEntry.Date = dateEntry;

            if (dateEntry != null)
            {
                var user = _context.User.SingleOrDefault(m => m.Id == dateEntry.UserId);

                UserEntry userEntry = new UserEntry();
                userEntry.Id = user.Id;
                userEntry.MobilePhone = user.MobilePhoneString;
                userEntry.DepartmentPhone = user.DepartmentPhoneString;
                userEntry.Username = user.Username;
                userEntry.Email = user.Email;
                userEntry.Role = user.Role.ToString();
                userEntry.NextShiftDate = null;
                todayEntry.User = userEntry;
            }

            return todayEntry;
        }

        // GET: api/Shift/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShift([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shift = await _context.Shift.SingleOrDefaultAsync(m => m.Id == id);

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        private bool ShiftExists(int id)
        {
            return _context.Shift.Any(e => e.Id == id);
        }
    }
}