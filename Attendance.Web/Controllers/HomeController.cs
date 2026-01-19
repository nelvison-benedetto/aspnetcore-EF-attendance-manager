using Attendance.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDayService _dayService;

        public HomeController(IDayService dayService)
        {
            _dayService = dayService;
        }

        // GET: /
        public async Task<ActionResult> Index()
        {
            // Ottieni il giorno corrente
            int todayId = await _dayService.GetOrCreateDayIdAsync(await _dayService.GetTodayAsync());
            // Reindirizza subito alla view del giorno di oggi
            return RedirectToAction("Day", "Attendance", new { dayId = todayId });
        }

        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Questa è la web app per la gestione delle presenze.";
            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}