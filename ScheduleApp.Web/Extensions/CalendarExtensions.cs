using System.Collections.Generic;
using System.Linq;
using ScheduleApp.Model;
using ScheduleApp.Web.Models;

namespace ScheduleApp.Web.Extensions
{
    public static class CalendarExtensions
    {
        public static List<CalendarViewModel> ToCalendarViewModelList(this List<Schedule> scheduleContext, string color = "green")
        {
            return scheduleContext.Select(schedule => new CalendarViewModel()
            {
                start = schedule?.Shift?.ShiftDate.GetValueOrDefault().Date,
                title = schedule?.User?.FirstName + ' ' + schedule?.User?.LastName,
                color = color,
                allDay = true
            }).ToList();
        }

        public static List<CalendarViewModel> ToCalendarViewModelList(this List<DatePreference> context, string color = "red")
        {
            return context.Select(s => new CalendarViewModel()
            {
                start = s?.Shift?.ShiftDate.GetValueOrDefault().Date,
                title = "preference",
                color = color,
                allDay = true
            }).ToList();
        }
    }
}
