using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Model.Models;

namespace ScheduleApp.Model
{
    public enum UserRole
    {
        Administrator = 0,
        Assistant = 1,
        Student = 2
    }
    public partial class User
    {
        public User()
        {
            DatePreference = new HashSet<DatePreference>();
            History = new HashSet<History>();
            Schedule = new HashSet<Schedule>();
            SwitchRequest = new HashSet<SwitchRequest>();
            SwitchshiftNewUser = new HashSet<SwitchShift>();
            SwitchshiftPrevUser = new HashSet<SwitchShift>();
            PendingRequests = new HashSet<PendingSwitch>();
            WishShiftRequests = new HashSet<SwitchRequest>();
            Statistics = new HashSet<Statistics>();
        }



        [HiddenInput]
        public int Id { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public UserRole Role { get; set; }
        [Display(Name = "Active?"), DefaultValue(true)]
        public bool IsActive { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.Date), Display(Name = "Last Login Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastLoginDate { get; set; }
        [DataType(DataType.PhoneNumber), Display(Name = "Mobile Phone")]
        public string MobilePhoneString { get; set; }
        [DataType(DataType.PhoneNumber), Display(Name = "Department Phone")]
        public string DepartmentPhoneString { get; set; }

        public ICollection<DatePreference> DatePreference { get; set; }
        public ICollection<History> History { get; set; }
        public ICollection<Schedule> Schedule { get; set; }
        public ICollection<SwitchRequest> SwitchRequest { get; set; }
        public ICollection<SwitchShift> SwitchshiftNewUser { get; set; }
        public ICollection<SwitchShift> SwitchshiftPrevUser { get; set; }
        public ICollection<PendingSwitch> PendingRequests { get; set; }
        public ICollection<SwitchRequest> WishShiftRequests { get; set; }
        public ICollection<Statistics> Statistics { get; set; }
    }
}
