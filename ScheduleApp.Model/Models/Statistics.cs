using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Model.Models
{
    public partial class Statistics
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Duration { get; set; }

        public User User { get; set; }
    }
}
