namespace ScheduleApp.Model
{
    public partial class SwitchRequest
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? CurrentShiftId { get; set; }
        public int? WishShiftId { get; set; }
        public bool? IsBroadcast { get; set; }
        public bool? HasBeenSwitched { get; set; }

        public Shift CurrentShift { get; set; }
        public User User { get; set; }
        public Shift WishShift { get; set; }
    }
}
