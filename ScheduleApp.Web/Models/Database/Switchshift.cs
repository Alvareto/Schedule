using System;

namespace ScheduleApp.Web.Models.Database
{
    public partial class SwitchShift
    {
        public int Id { get; set; }
        public int? PrevUserId { get; set; }
        public DateTime? PrevUserDate { get; set; }
        public int? NewUserId { get; set; }
        public DateTime? NewUserDate { get; set; }

        public Users NewUser { get; set; }
        public Users PrevUser { get; set; }
    }
}
