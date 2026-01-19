using System;
using System.Threading.Tasks;

namespace Attendance.Web.Services.Implementations
{
    public interface IDayService
    {
        Task<int> GetOrCreateDayIdAsync(DateTime date);
        Task<DateTime> GetTodayAsync();
    }
}