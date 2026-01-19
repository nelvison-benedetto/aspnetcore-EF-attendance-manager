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
    public class AttendanceService
    {

        public async Task<IList<AttendancePersonRowViewModel>> GetAttendanceForDayAsync(int dayId)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                var query = from p in db.Person
                            join a in db.Attendance
                                .Where(att => att.DayId == dayId)
                                on p.PersonId equals a.PersonId into pa
                            from att in pa.DefaultIfEmpty() // Left join
                            select new AttendancePersonRowViewModel
                            {
                                PersonId = p.PersonId,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                isAvailable = att != null ? (bool?)att.IsAvailable : null
                            };

                return await query.ToListAsync();
            }
        }

        public async Task AddOrUpdateAttendanceAsync(AttendanceInputModel input)
        {
            using (var db = new AttendanceDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                // Controlla se già esiste un record
                var existing = await db.Attendance
                    .FirstOrDefaultAsync(a => a.DayId == input.DayId && a.PersonId == input.PersonId);

                if (existing != null)
                {
                    // Aggiorna il record esistente
                    existing.IsAvailable = input.isAvailable;
                }
                else
                {
                    // Crea nuovo record
                    var newAttendance = new Attendance
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

                var personIds = inputs.Select(i => i.PersonId).ToList();

                // Carica in memoria i record esistenti per il giorno
                var existingRecords = await db.Attendance
                    .Where(a => a.DayId == dayId && personIds.Contains(a.PersonId))
                    .ToListAsync();

                foreach (var input in inputs)
                {
                    var existing = existingRecords.FirstOrDefault(a => a.PersonId == input.PersonId);
                    if (existing != null)
                    {
                        existing.IsAvailable = input.isAvailable;
                    }
                    else
                    {
                        db.Attendance.Add(new Models.Database.Attendance
                        {
                            DayId = dayId,
                            PersonId = input.PersonId,
                            IsAvailable = input.isAvailable
                        });
                    }
                }

                await db.SaveChangesAsync();
            }
        }

    }
}