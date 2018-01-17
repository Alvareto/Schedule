namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// otkaži zamjenu termina (akciju može izvesti samo korisnik koji je stvorio request ili admin)
    /// </summary>
    public class DeclinePendingSwitch
    {
        public int PendingSwitchId { get; set; }
        public int RequesterId { get; set; }
    }

}
