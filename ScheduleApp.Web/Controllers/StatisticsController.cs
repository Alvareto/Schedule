using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScheduleApp.Model;
using ScheduleApp.Web.Authorization;
using ScheduleApp.Web.Extensions;

namespace ScheduleApp.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ScheduleContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;

        public StatisticsController(ScheduleContext context, IAuthorizationService authorizationService, ILogger<StatisticsController> logger)
        {
            _context = context;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        private bool _isAuthorized => _authorizationService.AuthorizeAsync(User, _context.User, UserOperations.Mail).Result.Succeeded;

        // GET: Statistics
        public IActionResult Index()
        {
            if (!_isAuthorized)
            {
                return new ChallengeResult();
            }

            // User.Identity.Name: ipXXXXX@fer.hr
            List<Schedule> scheduleContext = _context.Schedule.Include(s => s.Shift).Include(s => s.User).ToList();

            var events = scheduleContext.ToCalendarViewModelList();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd"
            };

            ViewData["events"] = JsonConvert.SerializeObject(events, settings);

            return View();
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
