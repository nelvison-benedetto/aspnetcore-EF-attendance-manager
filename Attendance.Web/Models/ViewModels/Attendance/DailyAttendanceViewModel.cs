using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attendance.Web.Models.ViewModels.Attendance
{
    public class DailyAttendanceViewModel
    {
        public int DayId { get; set; }
        public DateTime AttendanceDate { get; set; }
        
        public IList<AttendancePersonRowViewModel> Persons { get; set; } = new List<AttendancePersonRowViewModel>();  //evita null reference exception (altrimenti in razor se fai un @foreach(){} error)!
        //IList<> better than List<> for flexibility(dependency injection priciple applicato anche alle views. no IEnumerable<T> xk quello va bene solo x lettura (ti serve usare Add() e indexes!))
    
    }
}