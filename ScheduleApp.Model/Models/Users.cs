using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Model
{
    public partial class User
    {
        public User()
        {
            DatePreference = new HashSet<DatePreference>();
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
        [Display(Name = "Active?")]
        [DefaultValue(false)]
        public bool? IsActive { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public ICollection<DatePreference> DatePreference { get; set; }
        public ICollection<History> History { get; set; }
        public ICollection<Schedule> Schedule { get; set; }
        public ICollection<SwitchRequest> Switchrequest { get; set; }
        public ICollection<SwitchShift> SwitchshiftNewUser { get; set; }
        public ICollection<SwitchShift> SwitchshiftPrevUser { get; set; }
    }
}
