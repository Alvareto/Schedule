namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// potvrda zamjene termina
    /// </summary>
    public class AcceptDirectPendingSwitch
    {
        /// <summary>
        /// id switch entryja
        /// </summary>
        public int SwitchRequestId { get; set; }
        /// <summary>
        /// id korisnika koji potvrđuje ponuđenu zamjenu zadanu switch entryjem (direct)
        /// </summary>
        public int AcceptorId { get; set; }
    }

}
