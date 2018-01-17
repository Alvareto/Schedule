using System;

namespace ScheduleApp.Web.Models.API
{
    public class BroadcastSwitchRequest
    {
        public int RequestUserId { get; set; }
        public DateTime OfferedDate { get; set; }
    }

}
