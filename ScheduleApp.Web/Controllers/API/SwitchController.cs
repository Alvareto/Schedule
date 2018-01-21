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
                    && pending.UserId == id
                    && pending.Status == ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW
                    select new PendingSwitchRequestEntry
                    {
                        SwitchRequestId = request.Id,
                        IsBroadcast = request.IsBroadcast,
                        RequestUserId = (int)request.User.Id,
                        RequesterUsername = request.User.Username,
                        RequestCreatedDate = request.RequestCreatedDate,
                        CurrentDate = (DateTime)request.CurrentShift.ShiftDate,
                        WishDate = request.WishShift.ShiftDate
                    }).ToList();
        }

        [HttpGet("RequesterPending/{id}")]
        public IEnumerable<PendingSwitchRequestEntry> GetReceivedPendingSwitchRequestEntriesForUser([FromRoute] int id)
        {
            return (from request in _context.SwitchRequest.AsQueryable()
                    where request.HasBeenSwitched == false 
                    && request.UserId == id
                    && (
                        from pending in request.PendingSwitches
                        where request.Id == pending.SwitchRequestId && pending.Status == ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW
                        select pending.SwitchRequestId
                       ).Contains(request.Id)
                    select new PendingSwitchRequestEntry
                    {
                        SwitchRequestId = request.Id,
                        IsBroadcast = request.IsBroadcast,
                        RequestUserId = (int)request.User.Id,
                        RequesterUsername = request.User.Username,                        
                        RequestCreatedDate = request.RequestCreatedDate,
                        CurrentDate = (DateTime)request.CurrentShift.ShiftDate,
                        WishDate = request.WishShift.ShiftDate
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


            var pendingSwitch = new PendingSwitch();
            pendingSwitch.UserId = directSwitchRequest.AcceptUserId;
            pendingSwitch.Date = directSwitchRequest.OfferedDate;
            pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW;


            var switchRequest = new SwitchRequest();
            switchRequest.UserWishShiftId = directSwitchRequest.AcceptUserId;
            switchRequest.HasBeenSwitched = false;
            switchRequest.UserId = directSwitchRequest.RequestUserId;
            switchRequest.CurrentShiftId = directSwitchRequest.RequesterShiftId;
            switchRequest.WishShiftId = directSwitchRequest.AcceptorShiftId;
            switchRequest.IsBroadcast = false;
            switchRequest.RequestCreatedDate = DateTime.Now;


            // Add 1 pending switch to notify wish-user about your current-shift-date switch request 
            switchRequest?.PendingSwitches?.Add(pendingSwitch);


            _context.Add(switchRequest);
            await _context.SaveChangesAsync();

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
            switchRequest.UserWishShiftId = null;
            switchRequest.IsBroadcast = true;
            switchRequest.RequestCreatedDate = DateTime.Now;

                // Add N pending switch to notify all users about your current-shift-date switch request 
                switchRequest.PendingSwitches = new List<PendingSwitch>();

            //  await _context.User.ForEachAsync(
            await _context.User.Where(u => u.Id != switchRequest.UserId).ForEachAsync(
                   u =>
                     {
                         switchRequest.PendingSwitches.Add(
                             new PendingSwitch()
                             {
                                 UserId = u.Id,
                                 Date = null,
                                 Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_NEW
                             });
                     });

                _context.Add(switchRequest);
                await _context.SaveChangesAsync();

            return Ok(true);
        }

        // POST: api/Switch/Acceptdirect
        [HttpPost("Acceptdirect")]
        public IActionResult PostSwitchDirectRequestAccept([FromBody] AcceptDirectPendingSwitch acceptPendingSwitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == acceptPendingSwitch.SwitchRequestId && ps.UserId == acceptPendingSwitch.AcceptorId);
            if (pendingSwitch == null)
            {
                return NotFound();
            }

            pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_ACCEPTED;

            var acceptor = _context.User.SingleOrDefault(u => u.Id == acceptPendingSwitch.AcceptorId);
            if (acceptor == null)
            {
                return NotFound();
            }

            var switchRequest = _context.SwitchRequest.SingleOrDefault(u => u.Id == acceptPendingSwitch.SwitchRequestId);
            if (switchRequest == null)
            {
                return NotFound();
            }

            switchRequest.HasBeenSwitched = true;

            var currentShift = _context.Shift.SingleOrDefault(u => u.Id == switchRequest.CurrentShiftId);
            var wishShift = _context.Shift.SingleOrDefault(u => u.Id == switchRequest.WishShiftId);

            var currentSchedule = _context.Schedule.SingleOrDefault(u => u.ShiftId == currentShift.Id);
            var wishSchedule = _context.Schedule.SingleOrDefault(u => u.ShiftId == wishShift.Id);

            var wishScheduleUserId = wishSchedule.UserId;
            wishSchedule.UserId = currentSchedule.UserId;
            currentSchedule.UserId = wishScheduleUserId;
           
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendingSwitch);
                    _context.Update(switchRequest);
                    _context.Update(currentSchedule);
                    _context.Update(wishSchedule);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }

            return Ok(true);
        }

        // POST: api/Switch/Acceptbroadcast
        [HttpPost("Acceptbroadcast")]
        public IActionResult PostSwitchBroadcastRequestAccept([FromBody] AcceptBroadcastPendingSwitch acceptPendingSwitch)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == acceptPendingSwitch.SwitchRequestId && ps.UserId == acceptPendingSwitch.AcceptorId);
            if (pendingSwitch == null)
            {
                return NotFound();
            }

            pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_ACCEPTED;

            var acceptor = _context.User.SingleOrDefault(u => u.Id == acceptPendingSwitch.AcceptorId);
            if (acceptor == null)
            {
                return NotFound();
            }

            var switchRequest = _context.SwitchRequest.SingleOrDefault(u => u.Id == acceptPendingSwitch.SwitchRequestId);
            if (switchRequest == null)
            {
                return NotFound();
            }
            switchRequest.HasBeenSwitched = true;

            var currentShift = _context.Shift.SingleOrDefault(u => u.Id == switchRequest.CurrentShiftId);
            var wishShift = _context.Shift.SingleOrDefault(u => u.Id == acceptPendingSwitch.OfferedShiftId);

            var currentSchedule = _context.Schedule.SingleOrDefault(u => u.ShiftId == currentShift.Id);
            var wishSchedule = _context.Schedule.SingleOrDefault(u => u.ShiftId == acceptPendingSwitch.OfferedShiftId);

            var wishScheduleUserId = wishSchedule.UserId;
            wishSchedule.UserId = currentSchedule.UserId;
            currentSchedule.UserId = wishScheduleUserId;
        
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(currentSchedule);
                    _context.Update(wishSchedule);
                    _context.Update(pendingSwitch);
                    _context.Update(switchRequest);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                        return NotFound();
                }
            }

            return Ok(true);
        }


        // POST: api/Switch/Decline
        [HttpPost("Decline")]
        public IActionResult PostSwitchRequestDecline([FromBody] DeclinePendingSwitch declinePendingSwitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pendingSwitch = _context.PendingSwitch.Single(ps => ps.SwitchRequestId == declinePendingSwitch.SwitchRequestId && ps.UserId == declinePendingSwitch.AcceptorId);
            pendingSwitch.Status = ScheduleApp.Web.Extensions.Constants.REQUEST_STATUS_REJECTED;
            try
            {
                _context.Update(pendingSwitch);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(true);
        }

        // DELETE: api/switch/delete
        [HttpPost("Delete")]
        public IActionResult DeleteSwitchRequest([FromBody] DeleteSwitchRequest deleteSwitchRequest)
        {
            var switchRequest =  _context.SwitchRequest.SingleOrDefault(m => m.Id == deleteSwitchRequest.SwitchRequestId);
            if (switchRequest.UserId == deleteSwitchRequest.UserId)
            {
                _context.SwitchRequest.Remove(switchRequest);
                _context.SaveChanges();
            }
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