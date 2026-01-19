using Attendance.Web.Models.Database;
using Attendance.Web.Models.ViewModels.Attendance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Attendance.Web.Services.Implementations
{
    //x more info check DayService.cs e PersonService.cs

    public class AttendanceService
    {
        public async Task<IList<AttendancePersonRowViewModel>> GetAttendanceForDayAsync(int dayId)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
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
                        isAvailable =  p.Attendance  //sfrutta le navigation props
                            .Where( a => a.DayId == dayId )
                                //solo fra le Attendence di QUESTO p, filtra per DaiId
                            .Select( a => (bool?)a.IsAvailable )
                            //LA QUERY ORA PRODUCE un bool, non piu un Attendance! '?' xk puo essere null se lo è vuoi value null
                            .FirstOrDefault()  //se esite una row Attendance return true or false, altrimenti null
                    })
                    .ToListAsync();
            }
        }

        public async Task AddOrUpdateAttendanceAsync(AttendanceInputModel input)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                var existingAttendance = await db.Attendance
                    .FirstOrDefaultAsync( a => a.DayId == input.DayId && a.PersonId == input.PersonId );

                if (existingAttendance != null)
                {
                    existingAttendance.IsAvailable = input.isAvailable;
                }
                else
                {
                    //##CREATE
                    var newAttendance = new Models.Database.Attendance
                    {
                        DayId = input.DayId,
                        PersonId = input.PersonId,
                        IsAvailable = input.isAvailable
                    };
                    db.Attendance.Add(newAttendance);
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateAttendanceBatchAsync(int dayId, IList<AttendanceInputModel> inputs)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                var personIds = inputs.Select( i => i.PersonId ).ToList(); //inputs è gia in memoria, quindi non si usa (inutile) async (.ToListAsync()) !

                //carica in memoria(RAM) i record esistenti per il giorno. è solo una select no crud
                var existingRecords = await db.Attendance
                    .Where( a => a.DayId == dayId && personIds.Contains(a.PersonId) )
                    .ToListAsync();

                foreach (var input in inputs)
                {
                    var existing = 
                        existingRecords.FirstOrDefault( a => a.PersonId == input.PersonId);
                        //!! cerca nella lista in memoria(in RAM), non fa query al db. result Attendence or null
                    if (existing != null)
                    {
                        existing.IsAvailable = input.isAvailable;  //aggiorna in RAM
                    }
                    else
                    {
                        //##CREATE
                        db.Attendance.Add(new Models.Database.Attendance
                        {
                            DayId = dayId,
                            PersonId = input.PersonId,
                            IsAvailable = input.isAvailable
                        });
                    }
                }
                await db.SaveChangesAsync();  //scrive sul db
            }
        }

    }
}