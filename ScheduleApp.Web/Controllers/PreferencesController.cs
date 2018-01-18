using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;
using ScheduleApp.Web.Models;

namespace ScheduleApp.Web.Controllers
{
    public class PreferencesController : Controller
    {
        private readonly ScheduleContext _context;

        public PreferencesController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: Preferences
        public async Task<IActionResult> Index()
        {
            var scheduleContext = _context.DatePreference.Include(d => d.Shift).Include(d => d.User);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: Preferences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.DatePreference
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }

            return View(datePreference);
        }

        // GET:  Users/Preferences/Date
        [HttpGet("Users/Preferences/Date")]
        public IActionResult Create(int? id)
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Users/5/Preferences/Date
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Users/Preferences/Date"), ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Day,IsPreffered")] DatePreferenceViewModel vm)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Email == User.Identity.Name);
            if (user == null)
            {
                return NotFound("User with given ID doesn't exist.");
            }

            var shift = await _context.Shift.SingleOrDefaultAsync(m => m.ShiftDate == vm.Day);
            if (shift == null)
            {
                return NotFound("Desired date preference doesn't have corresponding shift available.");
            }

            var datePreference = new DatePreference()
            {
                IsPreffered = vm.IsPreffered,
                ShiftId = shift.Id,
                UserId = user.Id
            };

            if (ModelState.IsValid)
            {
                _context.Add(datePreference);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }

            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", user.Id);
            return View(vm);
        }

        // GET: Preferences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.DatePreference.SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", datePreference.UserId);
            return View(datePreference);
        }

        // POST: Preferences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ShiftId,IsPreffered")] DatePreference datePreference)
        {
            if (id != datePreference.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datePreference);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatePreferenceExists(datePreference.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", datePreference.UserId);
            return View(datePreference);
        }

        // GET: Preferences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.DatePreference
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }

            return View(datePreference);
        }

        // POST: Preferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datePreference = await _context.DatePreference.SingleOrDefaultAsync(m => m.Id == id);
            _context.DatePreference.Remove(datePreference);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatePreferenceExists(int id)
        {
            return _context.DatePreference.Any(e => e.Id == id);
        }
    }
}
