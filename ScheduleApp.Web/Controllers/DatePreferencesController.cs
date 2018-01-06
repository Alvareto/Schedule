using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Web.Models;
using ScheduleApp.Web.Models.Database;

namespace ScheduleApp.Web.Controllers
{
    public class DatePreferencesController : Controller
    {
        private readonly ScheduleContext _context;

        public DatePreferencesController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: DatePreferences
        public async Task<IActionResult> Index()
        {
            var scheduleContext = _context.Datepreference.Include(d => d.User);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: DatePreferences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.Datepreference
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }

            return View(datePreference);
        }

        // GET: DatePreferences/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DatePreferences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ShiftId,IsPreffered")] DatePreference datePreference)
        {
            if (ModelState.IsValid)
            {
                _context.Add(datePreference);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", datePreference.UserId);
            return View(datePreference);
        }

        // GET: DatePreferences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.Datepreference.SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", datePreference.UserId);
            return View(datePreference);
        }

        // POST: DatePreferences/Edit/5
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", datePreference.UserId);
            return View(datePreference);
        }

        // GET: DatePreferences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datePreference = await _context.Datepreference
                .Include(d => d.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (datePreference == null)
            {
                return NotFound();
            }

            return View(datePreference);
        }

        // POST: DatePreferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datePreference = await _context.Datepreference.SingleOrDefaultAsync(m => m.Id == id);
            _context.Datepreference.Remove(datePreference);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatePreferenceExists(int id)
        {
            return _context.Datepreference.Any(e => e.Id == id);
        }
    }
}
