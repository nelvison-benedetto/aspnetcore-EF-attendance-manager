using Attendance.Web.Models.Database;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Web.Services.Implementations
{
    public class DayService
    {
        public async Task<int> GetOrCreateDayId(DateTime date) {
            using (var db = new AttendanceDbContext()) {
                db.Database.Log = (msg) => Console.WriteLine(msg);  //x debug
                DateTime day = date.Date; //x only day no hours ect
                var existingDay = await db.Day
                    .Where(d => DbFunctions.TruncateTime(d.AttendanceDate) == day)  //è d.AttendanceDate.Date == day, ma in modern EF si usa TruncateTime x ok sql
                    .Select(d => new { d.DayId})
                    .FirstOrDefaultAsync();

                if (existingDay != null) { 
                    return existingDay.DayId;
                }
                var newDay = new Day
                {
                    AttendanceDate = day
                };
                db.Day.Add(newDay);
                await db.SaveChangesAsync();
                return newDay.DayId;
            }
        }

        public Task<DateTime> GetToday() { 
            return Task.FromResult(DateTime.Today);
        }

    }
}