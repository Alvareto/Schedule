using System;
using System.Collections.Generic;

namespace ScheduleApp.Model
{
    public partial class HistoryShift
    {
        public HistoryShift()
        {
            History = new HashSet<History>();
        }

        public int Id { get; set; }
        public DateTime? Shiftdate { get; set; }

        public ICollection<History> History { get; set; }
    }
}
