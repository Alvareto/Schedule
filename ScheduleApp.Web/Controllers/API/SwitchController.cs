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
    [Route("api/Switch")]
    public class SwitchController : Controller
    {
        private readonly ScheduleContext _context;

        public SwitchController(ScheduleContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the history list of all done switch requests
        /// </summary>
        /// <returns></returns>
        [HttpGet("History")]
        public IEnumerable<SwitchRequest> History()
        {
            return _context.SwitchRequest.Where(s => s.HasBeenSwitched);
        }

        [HttpGet("Pending")]
        public IEnumerable<SwitchRequest> Pending()
        {
            return _context.SwitchRequest.Where(s => !s.HasBeenSwitched);
        }

        // POST: api/Switch/Direct
        [HttpPost("Direct")]
        public async Task<IActionResult> PostSwitchRequestDirect([FromBody] SwitchRequest switchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSwitchRequest", new { id = switchRequest.Id }, switchRequest);
        }

        // POST: api/Switch/Direct
        [HttpPost("Broadcast")]
        public async Task<IActionResult> PostSwitchRequestBroadcast([FromBody] SwitchRequest switchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSwitchRequest", new { id = switchRequest.Id }, switchRequest);
        }

        // POST: api/Switch/Accept
        [HttpPost("Accept")]
        public async Task<IActionResult> PostSwitchRequestAccept([FromBody] SwitchRequest switchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSwitchRequest", new { id = switchRequest.Id }, switchRequest);
        }

        // POST: api/Switch/Accept
        [HttpPost("Offer")]
        public async Task<IActionResult> PostSwitchRequestOffer([FromBody] SwitchRequest switchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSwitchRequest", new { id = switchRequest.Id }, switchRequest);
        }

        // POST: api/Switch/Accept
        [HttpPost("Decline")]
        public async Task<IActionResult> PostSwitchRequestDecline([FromBody] SwitchRequest switchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSwitchRequest", new { id = switchRequest.Id }, switchRequest);
        }

        // GET: api/Switch
        [HttpGet]
        public IEnumerable<SwitchRequest> GetSwitchRequest()
        {
            return _context.SwitchRequest;
        }

        private bool SwitchRequestExists(int id)
        {
            return _context.SwitchRequest.Any(e => e.Id == id);
        }
    }
}