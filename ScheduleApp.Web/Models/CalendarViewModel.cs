using System;

namespace ScheduleApp.Web.Models
{
    public class CalendarViewModel
    {
        public string title { get; set; }

        public DateTime? start { get; set; }

        public string color { get; set; }

        public bool allDay { get; set; }
    }
}
