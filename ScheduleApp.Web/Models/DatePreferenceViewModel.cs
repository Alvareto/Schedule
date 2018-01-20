using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models
{
    public class DatePreferenceViewModel
    {
        [Display(Name = "Date")]
        public DateTime Day { get; set; } // ShiftId
        [Display(Name = "Is Preffered?")]
        public bool IsPreffered { get; set; }
    }

    public class DatePreferenceEditViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Date"), DataType(DataType.Date)]
        public DateTime Day { get; set; } // ShiftId
        [Display(Name = "Is Preffered?")]
        public bool IsPreffered { get; set; }
    }
}
