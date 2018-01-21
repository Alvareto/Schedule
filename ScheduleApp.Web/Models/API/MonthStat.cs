namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// lista month statistika svih/jednog korisnika -> koliko je ukupno odrađeno sati smjena po mjesecima
    /// </summary>
    public class MonthStat
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ShiftTime { get; set; }
    }

}
