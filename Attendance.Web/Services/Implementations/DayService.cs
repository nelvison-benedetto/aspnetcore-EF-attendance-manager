using Attendance.Web.Models.Database;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Web.Services.Implementations
{
    public class DayService
    {
        public async Task<int> GetOrCreateDayIdAsync(DateTime date) {
            using (var db = new AttendanceDbContext()) {  //chiude auto connection fuori da blockscope
                db.Database.Log = (msg) => Console.WriteLine(msg);  //x debug
                DateTime day = date.Date; //x only day no hours ect
                var existingDay = await db.Day  //query LINQ pronta x essere eseguita
                     //Day è il dbset x tab day, permette di fare query come su una lista di objs
                    .Where(d => DbFunctions.TruncateTime(d.AttendanceDate) == day)
                        //d è ogni elemento del DbSet Day durante la query
                        //sarebbe come d.AttendanceDate.Date == day, ma in modern EF si usa TruncateTime altimenti invalid sql
                    .Select(d => new { d.DayId})
                    //per ogni elemento (filtrato da where) d del DbSet Day, crea un oggetto anonimo con una proprietà DayId
                    //non prende l'intera row, solo una column
                    .FirstOrDefaultAsync();  //get first row found or null
                //quindi ottieni il primo oggetto anonimo con DayId o null se non trovato

                if (existingDay != null) { 
                    return existingDay.DayId;  //return existing found dayId of target day
                }

                //##creation new Day
                var newDay = new Day  //temp instance, compili TUTTI i fields che sono NOT NULL sul db!!
                {
                    AttendanceDate = day
                };
                db.Day.Add(newDay);  //EF mette l’oggetto nello stato "Added"
                await db.SaveChangesAsync();  //fa INSERT nel db, DayId viene generato dal db
                return newDay.DayId;
            }
        }

        public Task<DateTime> GetTodayAsync()
        {  //non serve async xk non fa db(operations Input/Output), cmnq restituisce sempre un task
            return Task.FromResult(DateTime.Today);
            //"Ho un Task, ma non devo fare I/O, è già pronto subito"
        }
        //lo utilizzi e.g. DateTime today = await dayService.GetTodayAsync(); non fa nessuna operazione asincrona reale (nessun DB, file, rete), ma puoi comunque usare await! cool!

    }
}