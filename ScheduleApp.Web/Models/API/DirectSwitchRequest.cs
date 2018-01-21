using System;

namespace ScheduleApp.Web.Models.API
{
    public class DirectSwitchRequest
    {
        public int RequestUserId { get; set; }
        public int AcceptUserId { get; set; }
        public DateTime WantedDate { get; set; }
        public DateTime OfferedDate { get; set; }
        public int RequesterShiftId { get; set; }
        public int AcceptorShiftId { get; set; }
    }

}
