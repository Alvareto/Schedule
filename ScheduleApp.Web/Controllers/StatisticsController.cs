using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScheduleApp.Model;
using ScheduleApp.Web.Authorization;
using ScheduleApp.Web.Extensions;

namespace ScheduleApp.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ScheduleContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;

        public StatisticsController(ScheduleContext context, IAuthorizationService authorizationService, ILogger<StatisticsController> logger)
        {
            _context = context;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        private bool _isAuthorized => _authorizationService.AuthorizeAsync(User, _context.User, UserOperations.Mail).Result.Succeeded;

        // GET: Statistics
        public IActionResult Index()
        {
            if (!_isAuthorized)
            {
                return new ChallengeResult();
            }

            // User.Identity.Name: ipXXXXX@fer.hr
            List<Schedule> scheduleContext = _context.Schedule.Include(s => s.Shift).Include(s => s.User).ToList();

            var events = scheduleContext.ToCalendarViewModelList();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd"
            };

            ViewData["events"] = JsonConvert.SerializeObject(events, settings);

            return View();
        }

        // GET: Statistics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Shift)
                .Include(s => s.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Statistics/Create
        public IActionResult Create()
        {
            ViewData["ShiftId"] = new SelectList(_context.Shift, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Statistics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ShiftId")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShiftId"] = new SelectList(_context.Shift, "Id", "Id", schedule.ShiftId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", schedule.UserId);
            return View(schedule);
        }

        // GET: Statistics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.SingleOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["ShiftId"] = new SelectList(_context.Shift, "Id", "Id", schedule.ShiftId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", schedule.UserId);
            return View(schedule);
        }

        // POST: Statistics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ShiftId")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
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
            ViewData["ShiftId"] = new SelectList(_context.Shift, "Id", "Id", schedule.ShiftId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", schedule.UserId);
            return View(schedule);
        }

        // GET: Statistics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Shift)
                .Include(s => s.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Statistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.SingleOrDefaultAsync(m => m.Id == id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
