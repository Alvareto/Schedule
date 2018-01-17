using System;
using System.Collections.Generic;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// vraća sve aktivne nezavršene zahtjeve za zamjenom termina
    /// </summary>
    public class PendingSwitchEntry
    {
        public int SwitchId { get; set; }
        public int RequestUserId { get; set; }
        public string RequestUsername { get; set; }
        /// <summary>
        /// datum stvaranja ponude
        /// </summary>
        public DateTime RequesterOfferDate { get; set; }
        /// <summary>
        /// datum kojeg requestor želi mijenjati
        /// </summary>
        public DateTime RequesterDate { get; set; }
        /// <summary>
        /// lista svih ponuda za zamjenu
        /// </summary>
        List<DateEntry> OfferedDates { get; set; }
    }

}
