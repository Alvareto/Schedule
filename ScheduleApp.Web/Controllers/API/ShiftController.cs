using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;

namespace ScheduleApp.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Shift")]
    public class ShiftController : Controller
    {
        private readonly ScheduleContext _context;

        public ShiftController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: api/Shift
        [HttpGet("All")]
        public IEnumerable<Shift> GetShifts()
        {
            return _context.Shift;
        }

        // GET: api/Shift/Selection
        [HttpGet]
        public IEnumerable<Shift> Selection([FromRoute] DateTime intervalStartDate, [FromRoute] DateTime intervalEndDate)
        {
            return _context.Shift.Where(s => s.ShiftDate >= intervalStartDate && s.ShiftDate <= intervalEndDate);
        }

        // GET: api/Shift/User/5
        [HttpGet("User/{id}")]
        public IEnumerable<Shift> GetUserShifts([FromRoute] int id)
        {
            return _context.Schedule.Where(t => t.UserId == id).Select(s => s.Shift);
        }

        // GET: api/Shift/Today
        [HttpGet("Today")]
        public async Task<IActionResult> Today()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shift = await _context.Shift.SingleOrDefaultAsync(m => m.ShiftDate == DateTime.Today);

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
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