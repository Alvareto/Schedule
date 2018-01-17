using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models
{
    public class DatePreferenceViewModel
    {
        public DateTime Day { get; set; } // ShiftId
        public bool IsPreffered { get; set; }
    }
}
