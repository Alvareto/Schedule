namespace ScheduleApp.Model
{
    public partial class Schedule
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ShiftId { get; set; }

        public User User { get; set; }
        public Shift Shift { get; set; }
    }
}
