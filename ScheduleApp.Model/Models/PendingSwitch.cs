using System;
using System.Collections.Generic;

namespace ScheduleApp.Model
{
    public class PendingSwitch
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public SwitchRequest SwitchRequest { get; set; }
        public int? SwitchRequestId { get; set; }
    }
}