using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Model;
using ScheduleApp.Web.Models.API;

namespace ScheduleApp.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/User")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly ScheduleContext _context;

        public UserController(ScheduleContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets user information by email.
        /// </summary>
        // GET: api/User/email
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            UserEntry userEntry = new UserEntry();
            userEntry.Id = user.Id;
            userEntry.MobilePhone = "";
            userEntry.DepartmentPhone = "";
            userEntry.Username = user.Username;
            userEntry.Email = user.Email;
            userEntry.Role = user.Role;
            userEntry.NextShiftDate = null;

            return Json(userEntry);
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}