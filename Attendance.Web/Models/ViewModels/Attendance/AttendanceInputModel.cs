using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attendance.Web.Models.ViewModels.Attendance
{
    public class AttendanceInputModel
    {
        [Required]
        public int DayId { get; set; }
        [Required]
        public int PersonId { get; set; }
        [Required]
        public bool isAvailable { get; set; }

    }
}