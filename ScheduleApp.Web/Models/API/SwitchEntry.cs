using System;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// vraća povijest svih obavljenih zamjena
    /// </summary>
    public class SwitchEntry
    {
        public int Id { get; set; }
        public int RequestUserId { get; set; }
        public string RequesterUsername { get; set; }
        public DateTime RequesterDate { get; set; }
        public int AcceptUserId { get; set; }
        public string AcceptorUsername { get; set; }
        public DateTime AcceptorDate { get; set; }
        public DateTime SwitchConfirmedDate { get; set; }
    }

}
