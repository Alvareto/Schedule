using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models.API
{
    public class TodayEntry
    {
        public DateEntry Date { get; set; }
        public UserEntry User { get; set; }
    }
}
