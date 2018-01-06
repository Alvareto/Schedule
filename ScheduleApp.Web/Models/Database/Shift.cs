using System;
using System.Collections.Generic;

namespace ScheduleApp.Web.Models.Database
{
    public partial class Shift
    {
        public Shift()
        {
            SwitchrequestCurrentShift = new HashSet<SwitchRequest>();
            SwitchrequestWishShift = new HashSet<SwitchRequest>();
        }

        public int Id { get; set; }
        public DateTime? Shiftdate { get; set; }
        public bool? IsHoliday { get; set; }

        public ICollection<SwitchRequest> SwitchrequestCurrentShift { get; set; }
        public ICollection<SwitchRequest> SwitchrequestWishShift { get; set; }
    }
}
