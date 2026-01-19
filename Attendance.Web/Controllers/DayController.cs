using Attendance.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class DayController : Controller
    {
        private readonly IDayService _dayService;
        public DayController(IDayService dayService)
        {
            _dayService = dayService;
        }

        // GET: Day
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Day/Today
        public async Task<ActionResult> Today()
        {
            DateTime today = await _dayService.GetTodayAsync();
            int dayId = await _dayService.GetOrCreateDayIdAsync(today);
            return RedirectToAction("Details", new { id = dayId });
        }

        // GET: /Day/Details/5
        public async Task<ActionResult> Details(int id)
        {
            // qui potresti mostrare la view per il giorno specifico
            // ad esempio, passare dayId alla view per usare in AttendanceController
            ViewBag.DayId = id;
            return View();
        }

    }
}