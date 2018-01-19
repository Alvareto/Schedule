namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// negativan odgovor na primljeni switch request (direct ili broadcast)
    /// </summary>
    public class DeclinePendingSwitch
    {
        public int SwitchRequestId { get; set; }
        public int AcceptorId { get; set; }
    }

}
