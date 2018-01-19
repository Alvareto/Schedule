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

            var scheduleContext = _context.SwitchRequest.Include(s => s.CurrentShift).Include(s => s.User).Include(s => s.WishShift);
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
                .Include(s => s.User)
                .Include(s => s.WishShift)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (switchRequest == null)
            {
                return NotFound();
            }

            return View(switchRequest);
        }

        public IActionResult Accept(int id)
        {
            return null;
        }

        public IActionResult Reject(int id)
        {
            return null;
        }

        // GET: Requests/Create
        public IActionResult Create()
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
            ViewData["WishShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email != User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,CurrentShiftId,WishShiftId")] SwitchRequest switchRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(switchRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrentShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email == User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            ViewData["WishShiftId"] = new SelectList(_context.Schedule.Include(s => s.User).Where(s => s.User.Email != User.Identity.Name).Select(s => s.Shift).OrderBy(s => s.ShiftDate), "Id", "ShiftDate");
            ViewData["UserId"] = new SelectList(_context.User.Where(s => s.Email.Equals(User.Identity.Name)), "Id", "Email", switchRequest.UserId);
            return View(switchRequest);
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
