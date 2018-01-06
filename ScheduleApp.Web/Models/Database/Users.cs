using System.Collections.Generic;

namespace ScheduleApp.Web.Models.Database
{
    public partial class Users
    {
        public Users()
        {
            Datepreference = new HashSet<DatePreference>();
            History = new HashSet<History>();
            Schedule = new HashSet<Schedule>();
            Switchrequest = new HashSet<SwitchRequest>();
            SwitchshiftNewUser = new HashSet<SwitchShift>();
            SwitchshiftPrevUser = new HashSet<SwitchShift>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool? IsActive { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public ICollection<DatePreference> Datepreference { get; set; }
        public ICollection<History> History { get; set; }
        public ICollection<Schedule> Schedule { get; set; }
        public ICollection<SwitchRequest> Switchrequest { get; set; }
        public ICollection<SwitchShift> SwitchshiftNewUser { get; set; }
        public ICollection<SwitchShift> SwitchshiftPrevUser { get; set; }
    }
}
