using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// potvrda zamjene termina
    /// </summary>
    public class AcceptBroadcastPendingSwitch
    {
        /// <summary>
        /// id switch entryja
        /// </summary>
        public int SwitchRequestId { get; set; }
        /// <summary>
        /// id korisnika koji je ponudio datum za zamjenu (za taj switch entry)
        /// </summary>
        public int AcceptorId { get; set; }

        public int OfferedShiftId { get; set; }
    }
}
