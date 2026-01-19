using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Web.Models.ViewModels.Attendance
{
    public class AttendancePersonRowViewModel
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? isAvailable { get; set; }  //è gia di default un '= null;'

    }
}