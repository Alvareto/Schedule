using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleApp.Model
{
    public partial class SwitchRequest
    {
        public SwitchRequest()
        {
            PendingSwitch = new HashSet<PendingSwitch>();
        }

        [HiddenInput]
        public int Id { get; set; }
        [Display(Name = "User")]
        public int? UserId { get; set; }
        [Display(Name = "Current Shift")]
        public int? CurrentShiftId { get; set; }
        [Display(Name = "Desired Shift")]
        public int? WishShiftId { get; set; }
        [HiddenInput]
        [DefaultValue(false)]
        public bool IsBroadcast { get; set; }
        [HiddenInput]
        [DefaultValue(false)]
        public bool HasBeenSwitched { get; set; }

        public int? UserWishShiftId { get; set; }

        public User UserWishShift { get; set; }
        public ICollection<PendingSwitch> PendingSwitch { get; set; }

        [Display(Name = "Current Shift")]
        public Shift CurrentShift { get; set; }
        public User User { get; set; }
        [Display(Name = "Desired Shift")]
        public Shift WishShift { get; set; }
    }
}
