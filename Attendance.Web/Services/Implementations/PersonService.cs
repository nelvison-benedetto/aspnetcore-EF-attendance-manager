using Attendance.Web.Models.Database;
using Attendance.Web.Models.ViewModels.Attendance;
using Attendance.Web.Models.ViewModels.Person;
using Attendance.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Attendance.Web.Services.Implementations
{
    //x more info check DayService.cs

    public class PersonService : IPersonService
    {
        public async Task<IList<PersonViewModel>> GetAllPersonsAsync() {
            using (var db = new AttendanceDbContext()) {
                db.Database.Log = msg => Console.WriteLine(msg);
                return await db.Person
                    .Select( p => new PersonViewModel
                    { 
                        PersonId = p.PersonId,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                    })
                    //per ogni elemento (here no filtrato) p del DbSet Person, crea un oggetto concreto con property PersonId, FirstName, LastName
                    .ToListAsync();  //esegue la query sul db e ritorna la lista di oggetti concreti(che ti servono solo nel code, non sul db)
            }
        }

        public async Task<PersonViewModel> GetPersonByIdAsync(int personId)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                return await db.Person
                    .Where( p => p.PersonId == personId)
                    .Select( p => new PersonViewModel  //per ogni p filtrato, crea un obj concreto
                    {
                        PersonId = p.PersonId,
                        FirstName = p.FirstName,
                        LastName = p.LastName
                    })
                    .FirstOrDefaultAsync();  //legge al max 1 row da db, se trova la row allora cre obj concreto
            }
        }

        public async Task<IList<AttendancePersonRowViewModel>> GetPersonsForDayAsync(int dayId)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                //left join tra Person e Attendance per il giorno specifico
                //var query = 
                //    from p in db.Person
                //    join a in db.Attendance
                //        .Where(att => att.DayId == dayId)
                //        on p.PersonId equals a.PersonId into pa
                //    from att in pa.DefaultIfEmpty() // Left join
                //    select new AttendancePersonRowViewModel
                //    {
                //        PersonId = p.PersonId,
                //        FirstName = p.FirstName,
                //        LastName = p.LastName,
                //        isAvailable = att != null ? (bool?)att.IsAvailable : null
                //    };

                return await db.Person
                    .Select( p => new AttendancePersonRowViewModel
                    {
                        PersonId = p.PersonId,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        isAvailable =  p.Attendance  //sfrutta le navigation props!!(check entity EF generate dall’ .edmx)
                            .Where( a => a.DayId == dayId)  
                                //solo fra le Attendence di QUESTO p, filtra per DaiId
                            .Select( a => (bool?)a.IsAvailable)  //LA QUERY ORA PRODUCE un bool, non piu un Attendance!! '?' xk puo essere nulle se lo è vuoi value null
                            .FirstOrDefault()  
                                //se esite una row Attendance return true or false, altrimenti null
                    })
                    .ToListAsync();
            }
        }

        public async Task<int> AddPersonWithAttendanceAsync(CreatePersonAttendanceViewModel model)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                // 1️⃣ Recupero il Day esistente
                var day = await db.Day
                    .FirstOrDefaultAsync(d => d.DayId == model.DayId);

                if (day == null)
                    throw new Exception("Day non trovato");

                // 2️⃣ Creo la nuova Person
                var person = new Person
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                // 3️⃣ Creo l'Attendance e collego TUTTO con navigation properties
                var attendance = new Attendance
                {
                    Day = day,                // relazione con Day
                    Person = person,          // relazione con Person
                    IsAvailable = model.IsAvailable
                };

                // 4️⃣ Aggiungo SOLO la Person (EF capisce tutto il grafo)
                db.Person.Add(person);

                // 5️⃣ Salvo
                await db.SaveChangesAsync();

                // 6️⃣ Ritorno l'id della nuova Person
                return person.PersonId;
            }
        }

        public async Task UpdatePersonWithAttendanceAsync(UpdatePersonAttendanceViewModel model)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                // 1️⃣ Carico Person + Attendance
                var person = await db.Person
                    .Include(p => p.Attendance)
                    .FirstOrDefaultAsync(p => p.PersonId == model.PersonId);

                if (person == null)
                    throw new Exception("Person non trovata");

                // 2️⃣ Update dati Person
                person.FirstName = model.FirstName;
                person.LastName = model.LastName;

                // 3️⃣ Cerco l'Attendance per quel Day
                var attendance = person.Attendance
                    .FirstOrDefault(a => a.DayId == model.DayId);

                if (attendance != null)
                {
                    // 4️⃣ UPDATE Attendance esistente
                    attendance.IsAvailable = model.IsAvailable;
                }
                else
                {
                    // 5️⃣ INSERT nuova Attendance
                    attendance = new Attendance
                    {
                        PersonId = person.PersonId,
                        DayId = model.DayId,
                        IsAvailable = model.IsAvailable
                    };

                    person.Attendance.Add(attendance);
                }

                // 6️⃣ Save
                await db.SaveChangesAsync();
            }
        }

        //public async Task<int> AddPersonAsync(PersonViewModel person)
        //{
        //    using (var db = new AttendanceDbContext())
        //    {
        //        db.Database.Log = msg => Console.WriteLine(msg);
        //        var newPerson = new Person  //compila tutti i fields che sono NOT NULL sul db!
        //        {
        //            FirstName = person.FirstName,
        //            LastName = person.LastName
        //        };
        //        db.Person.Add(newPerson);
        //        await db.SaveChangesAsync();
        //        return newPerson.PersonId;
        //    }
        //}

        //public async Task UpdatePersonAsync(PersonViewModel person)
        //{
        //    using (var db = new AttendanceDbContext())
        //    {
        //        db.Database.Log = msg => Console.WriteLine(msg);
        //        var existingPerson = await db.Person
        //            .FirstOrDefaultAsync( p => p.PersonId == person.PersonId );
        //        if (existingPerson != null)
        //        {
        //            existingPerson.FirstName = person.FirstName;
        //            existingPerson.LastName = person.LastName;
        //            await db.SaveChangesAsync();
        //        }
                
        //    }
        //}


    }
}