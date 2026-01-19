using Attendance.Web.Models.ViewModels.Person;
using Attendance.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        // GET: Person
        public async Task<ActionResult> Index()
        {
            var persons = await _personService.GetAllPersonsAsync();
            return View(persons);
            //Views/{Controller}/{Action}.cshtml
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            return View();
            //serve solo x mostrare il form vuoto. non serve async o services
        }

        // POST: Person/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PersonViewModel model)
        {
            if(ModelState.IsValid)
            {
                await _personService.AddPersonAsync(model);
                return RedirectToAction("Index");
            }
            //altrimenti...
            return View(model);
        }

        // GET: Person/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null) return HttpNotFound();
            return View(person);
            //Views/{Controller}/{Action}.cshtml
        }

        // POST: Person/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _personService.UpdatePersonAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}