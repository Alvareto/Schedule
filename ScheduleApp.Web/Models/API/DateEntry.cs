using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models.API
{
    public class DateEntry
    {
        public DateTime Date { get; set; }
        public string DayType { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public int ShiftId { get; set; }
    }
}
