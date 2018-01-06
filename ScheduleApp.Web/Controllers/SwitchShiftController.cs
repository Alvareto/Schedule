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
    public class SwitchShiftController : Controller
    {
        private readonly ScheduleContext _context;

        public SwitchShiftController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: SwitchShift
        public async Task<IActionResult> Index()
        {
            var scheduleContext = _context.Switchshift.Include(s => s.NewUser).Include(s => s.PrevUser);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: SwitchShift/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchShift = await _context.Switchshift
                .Include(s => s.NewUser)
                .Include(s => s.PrevUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchShift == null)
            {
                return NotFound();
            }

            return View(switchShift);
        }

        // GET: SwitchShift/Create
        public IActionResult Create()
        {
            ViewData["NewUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PrevUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: SwitchShift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrevUserId,PrevUserDate,NewUserId,NewUserDate")] SwitchShift switchShift)
        {
            if (ModelState.IsValid)
            {
                _context.Add(switchShift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NewUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.NewUserId);
            ViewData["PrevUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.PrevUserId);
            return View(switchShift);
        }

        // GET: SwitchShift/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchShift = await _context.Switchshift.SingleOrDefaultAsync(m => m.Id == id);
            if (switchShift == null)
            {
                return NotFound();
            }
            ViewData["NewUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.NewUserId);
            ViewData["PrevUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.PrevUserId);
            return View(switchShift);
        }

        // POST: SwitchShift/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrevUserId,PrevUserDate,NewUserId,NewUserDate")] SwitchShift switchShift)
        {
            if (id != switchShift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(switchShift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SwitchShiftExists(switchShift.Id))
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
            ViewData["NewUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.NewUserId);
            ViewData["PrevUserId"] = new SelectList(_context.Users, "Id", "Id", switchShift.PrevUserId);
            return View(switchShift);
        }

        // GET: SwitchShift/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchShift = await _context.Switchshift
                .Include(s => s.NewUser)
                .Include(s => s.PrevUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchShift == null)
            {
                return NotFound();
            }

            return View(switchShift);
        }

        // POST: SwitchShift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var switchShift = await _context.Switchshift.SingleOrDefaultAsync(m => m.Id == id);
            _context.Switchshift.Remove(switchShift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SwitchShiftExists(int id)
        {
            return _context.Switchshift.Any(e => e.Id == id);
        }
    }
}
