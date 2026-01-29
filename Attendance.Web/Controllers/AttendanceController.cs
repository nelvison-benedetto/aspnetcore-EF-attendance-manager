using Attendance.Web.Models.ViewModels.Attendance;
using Attendance.Web.Services.Contracts;
using Attendance.Web.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // GET: Attendance
        public ActionResult Index()
        {
            return View();
            //Views/{Controller}/{Action}.cshtml
        }

        // GET: /Attendance/Day/5
        public async Task<ActionResult> Day(int dayId)
        {
            IList<AttendancePersonRowViewModel> attendance =
                await _attendanceService.GetAttendanceForDayAsync(dayId);
            ViewBag.DayId = dayId;
            return View(attendance);
            //Views/{Controller}/{Action}.cshtml
        }

        // POST: /Attendance/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int dayId, IList<AttendanceInputModel> inputs)
        {
            if (inputs != null)
            {
                await _attendanceService.AddOrUpdateAttendanceBatchAsync(dayId, inputs);
            }
            return RedirectToAction("Day", new { dayId });
        }

    }
}