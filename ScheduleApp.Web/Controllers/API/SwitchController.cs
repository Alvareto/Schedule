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
    [Route("api/Switch")]
    [AllowAnonymous]

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
        public IEnumerable<SwitchEntry> History()
        {
            return _context.SwitchShift.Select(s => new SwitchEntry
            {
                Id = s.Id,
                RequestUserId = (int)s.PrevUserId,
                RequesterDate = (DateTime)s.PrevUserDate,
                RequesterUsername = s.PrevUser.FirstName + " " + s.PrevUser.LastName,

                AcceptUserId = (int)s.NewUserId,
                AcceptorDate = (DateTime)s.NewUserDate,
                AcceptorUsername = s.NewUser.FirstName + " " + s.NewUser.LastName
            });
        }

        [HttpGet("Pending/{id}")]
        public IEnumerable<PendingSwitchRequestEntry> GetPendingSwitchRequestEntriesForUser([FromRoute] int id)
        {
            return (from request in _context.SwitchRequest.AsQueryable()
                         join pending in _context.PendingSwitch.AsQueryable()
                         on request.Id equals pending.SwitchRequestId
                         where request.HasBeenSwitched == false
                         select new PendingSwitchRequestEntry
                         {
                             SwitchRequestId = request.Id,
                             RequestUserId = (int)request.UserId,
                             RequesterUsername = request.User.Username,
                             RequestCreatedDate = null,
                             RequesterDate = (DateTime)request.CurrentShift.ShiftDate,
                             OfferedDates = request.PendingSwitch.Select(ps => new PendingSwitchEntry {
                                 Id = ps.Id,
                                 Status = ps.Status,
                                 UserId = ps.UserId,
                                 Date = ps.Date,
                                 SwitchRequestId = (int)ps.SwitchRequestId
                             }).ToList()
                         }).ToList();
        }

        [HttpGet("Receivedpending/{id}")]
        public IEnumerable<ReceivedPendingSwitchRequestEntry> GetReceivedPendingSwitchRequestEntriesForUser([FromRoute] int id)
        {
            return (from request in _context.SwitchRequest.AsQueryable()
                    join pending in _context.PendingSwitch.AsQueryable()
                    on request.Id equals pending.SwitchRequestId
                    where request.HasBeenSwitched == false
                    select new ReceivedPendingSwitchRequestEntry
                    {
                        SwitchRequestId = request.Id,
                        IsBroadcast = request.IsBroadcast,
                        RequesterUserId = (int)request.UserId,
                        RequesterUsername = request.User.Username,
                        RequestCreatedDate = null,
                        RequesterDate = (DateTime)request.CurrentShift.ShiftDate,
                    }).ToList();
        }

        // POST: api/Switch/Direct
        [HttpPost("Direct")]
        public async Task<IActionResult> PostSwitchRequestDirect([FromBody] DirectSwitchRequest directSwitchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            SwitchRequest switchRequest = new SwitchRequest();
            switchRequest.HasBeenSwitched = false;
            switchRequest.UserId = directSwitchRequest.RequestUserId;
            switchRequest.CurrentShiftId = directSwitchRequest.RequesterShiftId;
            switchRequest.WishShiftId = directSwitchRequest.AcceptorShiftId;
            switchRequest.IsBroadcast = false;

            _context.SwitchRequest.Add(switchRequest);
            var result = await _context.SaveChangesAsync();
            // pokreni proceduru za dodavanje zapisa u PendingSwitch za primatelja


            return Ok(true);
        }

        // POST: api/Switch/Broadcast
        [HttpPost("Broadcast")]
        public async Task<IActionResult> PostSwitchRequestBroadcast([FromBody] BroadcastSwitchRequest broadcastSwitchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            SwitchRequest switchRequest = new SwitchRequest();
            switchRequest.HasBeenSwitched = false;
            switchRequest.UserId = broadcastSwitchRequest.RequestUserId;
            switchRequest.CurrentShiftId = broadcastSwitchRequest.RequesterShiftId;
            switchRequest.IsBroadcast = true;

            _context.SwitchRequest.Add(switchRequest);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        // POST: api/Switch/Acceptdirect
        [HttpPost("Acceptdirect")]
        public async Task<IActionResult> PostSwitchDirectRequestAccept([FromBody] AcceptDirectPendingSwitch acceptPendingSwitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == acceptPendingSwitch.SwitchRequestId && ps.UserId == acceptPendingSwitch.AcceptorId);
            pendingSwitch.Status = "ACCEPTED";
            await _context.SaveChangesAsync();
            // TODO: pokreni proceduru zamjene shiftova

            return Ok(true);
        }

        // POST: api/Switch/Acceptbroadcast
        [HttpPost("Acceptbroadcast")]
        public async Task<IActionResult> PostSwitchBroadcastRequestAccept([FromBody] AcceptBroadcastPendingSwitch acceptPendingSwitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == acceptPendingSwitch.SwitchRequestId && ps.UserId == acceptPendingSwitch.AcceptorId);
            pendingSwitch.Status = "ACCEPTED";
            pendingSwitch.Date = acceptPendingSwitch.OfferedDate;
            await _context.SaveChangesAsync();

            return Ok(true);
        }


        // POST: api/Switch/Acceptbroadcast
        [HttpPost("Decline")]
        public async Task<IActionResult> PostSwitchRequestDecline([FromBody] DeclinePendingSwitch declinePendingSwitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == declinePendingSwitch.SwitchRequestId && ps.UserId == declinePendingSwitch.AcceptorId);
            pendingSwitch.Status = "DECLINED";
            await _context.SaveChangesAsync();

            return Ok(true);
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