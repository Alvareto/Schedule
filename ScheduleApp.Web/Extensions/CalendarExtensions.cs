using System.Collections.Generic;
using System.Linq;
using ScheduleApp.Model;
using ScheduleApp.Web.Models;

namespace ScheduleApp.Web.Extensions
{
    public static class CalendarExtensions
    {
        public static List<CalendarViewModel> ToCalendarViewModelList(this List<Schedule> scheduleContext)
        {
            return scheduleContext.Select(schedule => new CalendarViewModel()
            {
                start = (schedule?.Shift?.ShiftDate).GetValueOrDefault().Date,
                title = schedule?.User?.FirstName + ' ' + schedule?.User?.LastName,
                color = "green"
            }).ToList();
        }
    }
}
