using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Web.Models.ViewModels.Attendance
{
    public class CreatePersonAttendanceViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DayId { get; set; }
        public bool IsAvailable { get; set; }
    }

}