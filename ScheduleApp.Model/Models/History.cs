namespace ScheduleApp.Model
{
    public partial class History
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? HistoryShiftId { get; set; }

        public HistoryShift HistoryShift { get; set; }
        public User User { get; set; }
    }
}
