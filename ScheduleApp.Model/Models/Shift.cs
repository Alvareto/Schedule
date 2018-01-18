using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleApp.Model
{
    public partial class Shift
    {
        public Shift()
        {
            SwitchrequestCurrentShift = new HashSet<SwitchRequest>();
            SwitchrequestWishShift = new HashSet<SwitchRequest>();
            Templates = new HashSet<Schedule>();
            DatePreferences = new HashSet<DatePreference>();
        }

        [HiddenInput]
        public int Id { get; set; }
        public DateTime? ShiftDate { get; set; }
        public Boolean IsHoliday { get; set; }

        public ICollection<SwitchRequest> SwitchrequestCurrentShift { get; set; }
        public ICollection<Schedule> Templates { get; set; }
        public ICollection<SwitchRequest> SwitchrequestWishShift { get; set; }
        public ICollection<DatePreference> DatePreferences { get; set; }

    }
}
