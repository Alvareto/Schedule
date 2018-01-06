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
    public class HistoryController : Controller
    {
        private readonly ScheduleContext _context;

        public HistoryController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: History
        public async Task<IActionResult> Index()
        {
            var scheduleContext = _context.History.Include(h => h.HistoryShift).Include(h => h.User);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: History/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.History
                .Include(h => h.HistoryShift)
                .Include(h => h.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // GET: History/Create
        public IActionResult Create()
        {
            ViewData["HistoryShiftId"] = new SelectList(_context.Historyshift, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: History/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,HistoryShiftId")] History history)
        {
            if (ModelState.IsValid)
            {
                _context.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HistoryShiftId"] = new SelectList(_context.Historyshift, "Id", "Id", history.HistoryShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // GET: History/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.History.SingleOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }
            ViewData["HistoryShiftId"] = new SelectList(_context.Historyshift, "Id", "Id", history.HistoryShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // POST: History/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,HistoryShiftId")] History history)
        {
            if (id != history.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryExists(history.Id))
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
            ViewData["HistoryShiftId"] = new SelectList(_context.Historyshift, "Id", "Id", history.HistoryShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // GET: History/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.History
                .Include(h => h.HistoryShift)
                .Include(h => h.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // POST: History/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var history = await _context.History.SingleOrDefaultAsync(m => m.Id == id);
            _context.History.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(int id)
        {
            return _context.History.Any(e => e.Id == id);
        }
    }
}
