namespace ScheduleApp.Web.Models.Database
{
    public partial class History
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? HistoryShiftId { get; set; }

        public HistoryShift HistoryShift { get; set; }
        public Users User { get; set; }
    }
}
