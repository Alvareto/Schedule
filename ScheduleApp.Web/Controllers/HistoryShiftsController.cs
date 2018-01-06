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
    public class HistoryShiftsController : Controller
    {
        private readonly ScheduleContext _context;

        public HistoryShiftsController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: HistoryShifts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Historyshift.ToListAsync());
        }

        // GET: HistoryShifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyShift = await _context.Historyshift
                .SingleOrDefaultAsync(m => m.Id == id);
            if (historyShift == null)
            {
                return NotFound();
            }

            return View(historyShift);
        }

        // GET: HistoryShifts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HistoryShifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Shiftdate")] HistoryShift historyShift)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historyShift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(historyShift);
        }

        // GET: HistoryShifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyShift = await _context.Historyshift.SingleOrDefaultAsync(m => m.Id == id);
            if (historyShift == null)
            {
                return NotFound();
            }
            return View(historyShift);
        }

        // POST: HistoryShifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Shiftdate")] HistoryShift historyShift)
        {
            if (id != historyShift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historyShift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryShiftExists(historyShift.Id))
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
            return View(historyShift);
        }

        // GET: HistoryShifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyShift = await _context.Historyshift
                .SingleOrDefaultAsync(m => m.Id == id);
            if (historyShift == null)
            {
                return NotFound();
            }

            return View(historyShift);
        }

        // POST: HistoryShifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historyShift = await _context.Historyshift.SingleOrDefaultAsync(m => m.Id == id);
            _context.Historyshift.Remove(historyShift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryShiftExists(int id)
        {
            return _context.Historyshift.Any(e => e.Id == id);
        }
    }
}
