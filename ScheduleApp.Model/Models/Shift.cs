using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleApp.Model
{
    public partial class Shift
    {
        public Shift()
        {
            SwitchRequestCurrentShift = new HashSet<SwitchRequest>();
            SwitchRequestWishShift = new HashSet<SwitchRequest>();
            Templates = new HashSet<Schedule>();
            DatePreferences = new HashSet<DatePreference>();
        }

        [HiddenInput]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ShiftDate { get; set; }

        [Display(Name = "Holiday?")]
        [DefaultValue(false)]
        public Boolean? IsShorterDay { get; set; }

        public ICollection<SwitchRequest> SwitchRequestCurrentShift { get; set; }
        public ICollection<Schedule> Templates { get; set; }
        public ICollection<SwitchRequest> SwitchRequestWishShift { get; set; }
        public ICollection<DatePreference> DatePreferences { get; set; }

    }
}
