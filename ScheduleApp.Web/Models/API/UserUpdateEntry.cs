namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// update informacija o korisniku
    /// </summary>
    public class UserUpdateEntry
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string MobilePhone { get; set; }
        public string DepartmentPhone { get; set; }
    }

}
