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
    public class SwitchRequestsController : Controller
    {
        private readonly ScheduleContext _context;

        public SwitchRequestsController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: SwitchRequests
        public async Task<IActionResult> Index()
        {
            var scheduleContext = _context.Switchrequest.Include(s => s.CurrentShift).Include(s => s.User).Include(s => s.WishShift);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: SwitchRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.Switchrequest
                .Include(s => s.CurrentShift)
                .Include(s => s.User)
                .Include(s => s.WishShift)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }

            return View(switchRequest);
        }

        // GET: SwitchRequests/Create
        public IActionResult Create()
        {
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate");
            return View();
        }

        // POST: SwitchRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,CurrentShiftId,WishShiftId,IsBroadcast,HasBeenSwitched")] SwitchRequest switchRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(switchRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.CurrentShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", switchRequest.UserId);
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.WishShiftId);
            return View(switchRequest);
        }

        // GET: SwitchRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.Switchrequest.SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.CurrentShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", switchRequest.UserId);
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.WishShiftId);
            return View(switchRequest);
        }

        // POST: SwitchRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CurrentShiftId,WishShiftId,IsBroadcast,HasBeenSwitched")] SwitchRequest switchRequest)
        {
            if (id != switchRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(switchRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SwitchRequestExists(switchRequest.Id))
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
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.CurrentShiftId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", switchRequest.UserId);
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "Shiftdate", switchRequest.WishShiftId);
            return View(switchRequest);
        }

        // GET: SwitchRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.Switchrequest
                .Include(s => s.CurrentShift)
                .Include(s => s.User)
                .Include(s => s.WishShift)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }

            return View(switchRequest);
        }

        // POST: SwitchRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var switchRequest = await _context.Switchrequest.SingleOrDefaultAsync(m => m.Id == id);
            _context.Switchrequest.Remove(switchRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SwitchRequestExists(int id)
        {
            return _context.Switchrequest.Any(e => e.Id == id);
        }
    }
}
