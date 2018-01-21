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
using ScheduleApp.Web.Extensions;

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
                .Include(s => s.PendingSwitches)
                .Where(s => s.User.Email.Equals(User.Identity.Name) || s.UserWishShift.Email.Equals(User.Identity.Name)) // one of the users is involved
                ;

            var myRequests = scheduleContext.Where(s => s.User.Email.Equals(User.Identity.Name));
            var requestsToMe = scheduleContext.Where(s => s.UserWishShift.Email.Equals(User.Identity.Name))
                .Where(s => !s.HasBeenSwitched); // bilo direct bilo broadcast koji nije zamijenjen

            //var myDirect = myRequests.Where(s => !s.IsBroadcast);
            //var myBroadcast = myRequests.Where(s => s.IsBroadcast);
            //var directToMe = requestsToMe.Where(s => !s.IsBroadcast);
            //var broadcastToMe = requestsToMe.Where(s => s.IsBroadcast);

            var model =
                new Dictionary<RequestType, List<SwitchRequest>>
                {
                    { RequestType.MY_REQUESTS, await myRequests.ToListAsync() },
                    { RequestType.REQUESTS_TO_ME, await requestsToMe.ToListAsync() }
                    //{ RequestType.directToMe, await directToMe.ToListAsync() },
                    //{ RequestType.broadcastToMe, await broadcastToMe.ToListAsync() }
                };

            return View(model); //await scheduleContext.ToListAsync());
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
            var pendingSwitch = _context.PendingSwitch
                .Include(s => s.User)
                .Include(s => s.SwitchRequest) //.ThenInclude(m => m.WishShift)
                .SingleOrDefault(s => s.Id == id);

            if (pendingSwitch == null)
            {
                return NotFound();
            }

            pendingSwitch.Status = Extensions.Constants.REQUEST_STATUS_ACCEPTED;

            var switchRequest = _context.SwitchRequest
                .Include(s => s.User)
                .Include(s => s.CurrentShift)
                .Include(s => s.WishShift)
                .Include(s => s.UserWishShift)
                .Include(s => s.PendingSwitches)
                .SingleOrDefault(s => s.Id == pendingSwitch.SwitchRequestId);
            if (switchRequest == null)
            {
                return NotFound();
            }

            //pendingSwitch.SwitchRequest.HasBeenSwitched = true;
            if (switchRequest.IsBroadcast)
            {
                // if it's broadcast, reject all other pendingSwitches
                foreach (var p in switchRequest.PendingSwitches)
                {
                    p.Status = Extensions.Constants.REQUEST_STATUS_REJECTED;
                }
                // and set WishShift to acceptor random(?) shift
                // -- get acceptor
                var acceptor = _context.User.SingleOrDefault(u => u.Email.Equals(User.Identity.Name));
                if (acceptor == null)
                {
                    return NotFound();
                }

                switchRequest.UserWishShift = acceptor;

                // -- get random(?) shift
                var acceptorSchedule = _context.Schedule.Include(u => u.Shift).Include(u => u.User)
                    .FirstOrDefault(u => u.User.Id == acceptor.Id);
                if (acceptorSchedule == null)
                {
                    return NotFound();
                }

                switchRequest.WishShift = acceptorSchedule.Shift;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendingSwitch);
                    _context.Update(switchRequest);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PendingSwitchExists(pendingSwitch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Reject(int id)
        {
            var pendingSwitch = _context.PendingSwitch
                .Include(s => s.User)
                .Include(s => s.SwitchRequest)
                .SingleOrDefault(s => s.Id == id);

            if (pendingSwitch == null)
            {
                return NotFound();
            }



            pendingSwitch.Status = Extensions.Constants.REQUEST_STATUS_REJECTED;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendingSwitch);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PendingSwitchExists(pendingSwitch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Requests/Broadcast
        public IActionResult Broadcast()
        {
            var user = _context.User.SingleOrDefault(s => s.Email.Equals(User.Identity.Name));
            var switchRequest = new SwitchRequest();
            switchRequest.UserId = user?.Id;
            switchRequest.PendingSwitches = new List<PendingSwitch>();
            switchRequest.IsBroadcast = true;

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            return View(switchRequest);
        }

        // POST: Requests/Broadcast
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Broadcast([Bind("Id,UserId,CurrentShiftId,RequestCreatedDate,PendingSwitches")] SwitchRequest vm)
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

            // Add N pending switch to notify all users about your current-shift-date switch request 
            var pendingSwitches = await _context.PendingSwitch.Where(m => m.SwitchRequestId == vm.Id).ToListAsync();
            //if (pendingSwitches.Any())
            var users = _context.User.Where(u => u.Email != user.Email && u.IsActive).ToList();
            foreach (var u in users)
            {
                var pendingSwitch = new PendingSwitch();
                pendingSwitch.UserId = u.Id; // for each valid users
                pendingSwitch.Date = currentShift?.ShiftDate;
                pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW;

                pendingSwitches.Add(pendingSwitch);
            }

            //if (ModelState.IsValid)
            //{
            //    _context.Add(pendingSwitch);
            //    await _context.SaveChangesAsync();
            //}

            var switchRequest = new SwitchRequest()
            {
                IsBroadcast = true,
                HasBeenSwitched = false,
                CurrentShift = currentShift,
                User = user,
                RequestCreatedDate = DateTime.Now
            };

            // Add N pending switch to notify all users about your current-shift-date switch request 
            foreach (var pendingSwitch in pendingSwitches)
            {
                switchRequest.PendingSwitches?.Add(pendingSwitch);
            }

            if (ModelState.IsValid)
            {

                _context.Add(switchRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate", vm.CurrentShiftId);
            //ViewData["UserId"] = new SelectList(_context.User.Where(s => s.Email.Equals(User.Identity.Name)), "Id", "Email", vm.UserId);

            return View(vm);
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

        private bool PendingSwitchExists(int id)
        {
            return _context.PendingSwitch.Any(e => e.Id == id);
        }
    }
}
