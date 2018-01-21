using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models.API
{
    public class PendingSwitchEntry
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int SwitchRequestId { get; set; }
    }
}
