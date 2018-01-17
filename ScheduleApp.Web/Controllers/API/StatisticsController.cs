using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;

namespace ScheduleApp.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Statistics")]
    public class StatisticsController : Controller
    {
        private readonly ScheduleContext _context;

        public StatisticsController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: api/Statistics/All
        [HttpGet("All")]
        public IEnumerable<Schedule> GetAllStats()
        {
            return _context.Schedule;
        }

        // GET: api/Statistics/User/5
        [HttpGet("User/{id}")]
        public IEnumerable<Schedule> GetUserStats([FromRoute] int id)
        {
            return _context.Schedule;
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}