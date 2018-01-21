using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;
using ScheduleApp.Model.Models;
using ScheduleApp.Web.Models.API;

namespace ScheduleApp.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Statistics")]
    [AllowAnonymous]
    public class StatisticsController : Controller
    {
        private readonly ScheduleContext _context;

        public StatisticsController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: api/Statistics/All
        [HttpGet("All")]
        public IEnumerable<Statistics> GetAllStats()
        {
            return _context.Statistics.ToList();
        }

        // GET: api/Statistics/User/5
        [HttpGet("User/{id}")]
        public IEnumerable<MonthStat> GetUserStats([FromRoute] int id)
        {
            return (from stat in _context.Statistics.AsQueryable()
                    join user in _context.User.AsQueryable()
                    on stat.UserId equals user.Id
                    where stat.Year == DateTime.Now.Year
                    select new MonthStat
                    {
                        Year = (int)stat.Year,
                        Month = (int)stat.Month,
                        ShiftTime = (int)stat.Duration
                    }).ToList();
        }
    }
}