using ScheduleApp.Model;
using System;
using System.Collections.Generic;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// vraća sve aktivne nezavršene zahtjeve za zamjenom termina
    /// </summary>
    public class ReceivedPendingSwitchRequestEntry
    {
        public int SwitchRequestId { get; set; }
        public int RequesterUserId { get; set; }
        public string RequesterUsername { get; set; }
        public bool IsBroadcast { get; set; }
        /// <summary>
        /// datum stvaranja ponude
        /// </summary>
        public DateTime? RequestCreatedDate { get; set; }
        /// <summary>
        /// datum kojeg requestor želi mijenjati
        /// </summary>
        public DateTime RequesterDate { get; set; }
    }

}
