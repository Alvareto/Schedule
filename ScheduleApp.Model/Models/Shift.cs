using System;
using System.Collections.Generic;

namespace ScheduleApp.Model
{
    public partial class Shift
    {
        public Shift()
        {
            SwitchrequestCurrentShift = new HashSet<SwitchRequest>();
            SwitchrequestWishShift = new HashSet<SwitchRequest>();
            Templates = new HashSet<Schedule>();
        }

        public int Id { get; set; }
        public DateTime? ShiftDate { get; set; }
        public bool? IsHoliday { get; set; }

        public ICollection<SwitchRequest> SwitchrequestCurrentShift { get; set; }
        public ICollection<Schedule> Templates { get; set; }
        public ICollection<SwitchRequest> SwitchrequestWishShift { get; set; }
    }
}
