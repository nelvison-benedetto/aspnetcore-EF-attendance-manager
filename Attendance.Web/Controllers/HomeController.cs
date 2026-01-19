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
            //auto injection by mvc, check file App_Start/UnityConfig.cs x more
        {
            _dayService = dayService;
        }

        // GET: /
        public async Task<ActionResult> Index()
        {
            // Ottieni il giorno corrente
            int todayId = await _dayService.GetOrCreateDayIdAsync( await _dayService.GetTodayAsync() );
            // Reindirizza subito alla view del giorno di oggi
            return RedirectToAction(
                "Day",          //actionName (nome del method nel controller di destinazione)
                "Attendance",   //controllerName (controlle di destinazione ...Controller)
                new { dayId = todayId }  //routeValues (params passati al method)
            );
        }


        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Questa è la web app per la gestione delle presenze.";
            return View();  
            //MVC cerca auto view razor /Views/{Controller}/{Action}.cshtml
            //quindi .../About.cshtml
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}