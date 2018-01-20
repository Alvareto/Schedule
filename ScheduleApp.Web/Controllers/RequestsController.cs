using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScheduleApp.Model;
using ScheduleApp.Web.Authorization;

namespace ScheduleApp.Web.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ScheduleContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;

        public RequestsController(ScheduleContext context, IAuthorizationService authorizationService, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        private bool _isAuthorized => _authorizationService.AuthorizeAsync(User, _context.User, UserOperations.Mail).Result.Succeeded;

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            if (!_isAuthorized)
            {
                return new ChallengeResult();
            }

            var scheduleContext = _context.SwitchRequest
                .Include(s => s.CurrentShift)
                .Include(s => s.WishShift)
                .Include(s => s.User)
                .Include(s => s.UserWishShift)
                .Include(s => s.PendingSwitches);
            return View(await scheduleContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.SwitchRequest
                .Include(s => s.CurrentShift)
                .Include(s => s.WishShift)
                .Include(s => s.User)
                .Include(s => s.UserWishShift)
                .Include(s => s.PendingSwitches)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }

            return View(switchRequest);
        }

        public IActionResult Accept(int id)
        {
            var switchRequest = _context.PendingSwitch.SingleOrDefault(s => s.Id == id);

            //switchRequest.
            return null;
        }

        public IActionResult Reject(int id)
        {
            return null;
        }

        // GET: Requests/Broadcast
        public IActionResult Broadcast()
        {
            var user = _context.User.SingleOrDefault(s => s.Email.Equals(User.Identity.Name));
            if (user == null)
            {
                ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            }
            else
            {
                ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", user.Id);
            }

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            return View();
        }

        // POST: Requests/Broadcast
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Broadcast([Bind("Id,UserId,CurrentShiftId,PendingSwitch,IsBroadcast")] SwitchRequest switchRequest)
        {
            if (ModelState.IsValid)
            {
                // Add N pending switch to notify all users about your current-shift-date switch request 
                switchRequest.PendingSwitches = new List<PendingSwitch>();

                await _context.User.Where(u => u.Id != switchRequest.UserId).ForEachAsync(
                     u =>
                     {
                         switchRequest.PendingSwitches.Add(
                             new PendingSwitch()
                             {
                                 UserId = u.Id,
                                 Date = switchRequest.CurrentShift.ShiftDate,
                                 Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW
                             });
                     });

                switchRequest.IsBroadcast = true;

                _context.Add(switchRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            ViewData["UserId"] = new SelectList(_context.User.Where(s => s.Email.Equals(User.Identity.Name)), "Id", "Email", switchRequest.UserId);

            return View(switchRequest);
        }


        // GET: Requests/Create
        public IActionResult Create()
        {
            var user = _context.User.SingleOrDefault(s => s.Email.Equals(User.Identity.Name));
            var switchRequest = new SwitchRequest();
            switchRequest.UserId = user?.Id;
            switchRequest.PendingSwitches = new List<PendingSwitch>();

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            ViewData["WishShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email != User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            return View(switchRequest);
        }

        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,CurrentShiftId,WishShiftId,RequestCreatedDate,PendingSwitches")] SwitchRequest vm)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == vm.UserId);
            if (user == null)
            {
                user = _context.User.SingleOrDefault(s => s.Email.Equals(User.Identity.Name));
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "User with given ID doesn't exist.");
                }
            }

            var currentShift = await _context.Shift.SingleOrDefaultAsync(m => m.Id == vm.CurrentShiftId);
            if (currentShift == null)
            {
                ModelState.AddModelError("NotFound", "Current shift with given ID doesn't exist.");
            }

            var wishShift = await _context.Shift.SingleOrDefaultAsync(m => m.Id == vm.WishShiftId);
            if (wishShift == null)
            {
                ModelState.AddModelError("NotFound", "Wish shift with given ID doesn't exist.");
            }

            var wishShiftUser = await _context.Schedule.Include(m => m.User).Where(s => s.ShiftId == wishShift.Id)
                .Select(s => s.User).SingleOrDefaultAsync();
            if (wishShiftUser == null)
            {
                ModelState.AddModelError("NotFound", "Wish shift user with given ID doesn't exist.");
            }

            var pendingSwitch = await _context.PendingSwitch.SingleOrDefaultAsync(m => m.SwitchRequestId == vm.Id);
            if (pendingSwitch == null)
            {
                pendingSwitch = new PendingSwitch();
                pendingSwitch.UserId = wishShiftUser?.Id;
                pendingSwitch.Date = currentShift?.ShiftDate;
                pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW;
            }

            //if (ModelState.IsValid)
            //{
            //    _context.Add(pendingSwitch);
            //    await _context.SaveChangesAsync();
            //}

            var switchRequest = new SwitchRequest()
            {
                IsBroadcast = false,
                HasBeenSwitched = false,
                CurrentShift = currentShift,
                WishShift = wishShift,
                UserWishShift = wishShiftUser,
                User = user,
                RequestCreatedDate = DateTime.Now
            };
            // Add 1 pending switch to notify wish-user about your current-shift-date switch request 
            switchRequest?.PendingSwitches?.Add(pendingSwitch);

            if (ModelState.IsValid)
            {
                _context.Add(switchRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            ViewData["WishShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email != User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            //ViewData["UserId"] = new SelectList(_context.User.Where(s => s.Email.Equals(User.Identity.Name)), "Id", "Email", switchRequest.UserId);

            return View(vm);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.SwitchRequest.SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "ShiftDate", switchRequest.CurrentShiftId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", switchRequest.UserId);
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "ShiftDate", switchRequest.WishShiftId);
            return View(switchRequest);
        }

        // POST: Requests/Edit/5
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
            ViewData["CurrentShiftId"] = new SelectList(_context.Shift, "Id", "ShiftDate", switchRequest.CurrentShiftId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", switchRequest.UserId);
            ViewData["WishShiftId"] = new SelectList(_context.Shift, "Id", "ShiftDate", switchRequest.WishShiftId);
            return View(switchRequest);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var switchRequest = await _context.SwitchRequest
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

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var switchRequest = await _context.SwitchRequest.SingleOrDefaultAsync(m => m.Id == id);
            _context.SwitchRequest.Remove(switchRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SwitchRequestExists(int id)
        {
            return _context.SwitchRequest.Any(e => e.Id == id);
        }
    }
}
