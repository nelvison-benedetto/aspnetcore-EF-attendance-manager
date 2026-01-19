using Attendance.Web.Models.ViewModels.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Web.Services.Contracts
{
    public interface IAttendanceService
    {

        Task<IList<AttendancePersonRowViewModel>> GetAttendanceForDayAsync(int dayId);
        Task AddOrUpdateAttendanceAsync(AttendanceInputModel input);
        Task AddOrUpdateAttendanceBatchAsync(int dayId, IList<AttendanceInputModel> inputs);

    }
}
