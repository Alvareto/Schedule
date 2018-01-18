using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScheduleApp.Model;
using ScheduleApp.Web.Authorization;
using ScheduleApp.Web.Extensions;
using ScheduleApp.Web.Models;

namespace ScheduleApp.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ScheduleContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        //private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ScheduleContext context, IAuthorizationService authorizationService, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        private bool _isAuthorized => _authorizationService.AuthorizeAsync(User, _context.User, UserOperations.Mail).Result.Succeeded;

        public IActionResult Index()
        {
            if (!_isAuthorized)
            {
                return new UnauthorizedResult();
            }

            ViewData["events"] = GetUserScheduleEvents();
            ViewData["preferences"] = GetUserPreferenceEvents();

            ViewData["CountPreferences"] = _context.DatePreference.Count();
            ViewData["CountRequests"] = _context.SwitchRequest.Count();

            return View();
        }

        private string GetUserScheduleEvents()
        {
            // User.Identity.Name: ipXXXXX@fer.hr
            List<Schedule> scheduleContext = _context.Schedule.Include(s => s.Shift).Include(s => s.User).Where(s => string.Equals(s.User.Email, User.Identity.Name)).ToList();

            var events = scheduleContext.ToCalendarViewModelList();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd"
            };
            return JsonConvert.SerializeObject(events, settings);
        }

        private string GetUserPreferenceEvents()
        {
            // User.Identity.Name: ipXXXXX@fer.hr
            List<DatePreference> scheduleContext = _context.DatePreference.Include(s => s.Shift).Include(s => s.User).Where(s => string.Equals(s.User.Email, User.Identity.Name)).ToList();

            var events = scheduleContext.ToCalendarViewModelList();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd"
            };

            return JsonConvert.SerializeObject(events, settings);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
