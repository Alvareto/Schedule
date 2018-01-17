using System;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// ponuda datuma zamjene termina
    /// </summary>
    public class OfferDateForPendingSwitch
    {
        /// <summary>
        /// id switch entryja
        /// </summary>
        public int SwitchId { get; set; }
        /// <summary>
        /// id korisnika koji nudi Datum za zamjenu
        /// </summary>
        public int AcceptorId { get; set; }
        public DateTime OfferedDate { get; set; }
    }

}
