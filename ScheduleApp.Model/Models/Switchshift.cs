using System;
using System.Collections.Generic;

namespace ScheduleApp.Model
{
    public partial class SwitchShift
    {
        public int Id { get; set; }
        public int? PrevUserId { get; set; }
        public DateTime? PrevUserDate { get; set; }
        public int? NewUserId { get; set; }
        public DateTime? NewUserDate { get; set; }

        public User NewUser { get; set; }
        public User PrevUser { get; set; }
    }
}
