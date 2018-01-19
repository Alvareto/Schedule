using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Model
{
    public partial class DatePreference
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ShiftId { get; set; }
        [Display(Name="Is preffered?")]
        [DefaultValue(false)]
        public bool IsPreffered { get; set; }

        public User User { get; set; }
        public Shift Shift { get; set; }
    }
}
