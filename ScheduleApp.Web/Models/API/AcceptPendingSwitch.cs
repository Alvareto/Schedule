namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// potvrda zamjene termina
    /// </summary>
    public class AcceptPendingSwitch
    {
        /// <summary>
        /// id switch entryja
        /// </summary>
        public int PendingSwitchId { get; set; }
        /// <summary>
        /// id korisnika koji je ponudio datum za zamjenu (za taj switch entry)
        /// </summary>
        public int AcceptorId { get; set; }
        /// <summary>
        /// id korisnika koji je pokrenuo proces zamjene (direktni switch ili broadcast)
        /// </summary>
        public int RequesterId { get; set; }
    }

}
