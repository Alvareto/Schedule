using System;

namespace ScheduleApp.Web.Models.API
{
    /// <summary>
    /// update informacija o korisniku
    /// </summary>
    public class UserEntry
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string DepartmentPhone { get; set; }
        public DateTime? NextShiftDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

    }
}
