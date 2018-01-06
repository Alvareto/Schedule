namespace ScheduleApp.Web.Models.Database
{
    public partial class Schedule
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ShiftId { get; set; }

        public Users User { get; set; }
    }
}
