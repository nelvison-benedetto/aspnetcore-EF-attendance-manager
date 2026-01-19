using Attendance.Web.Models.ViewModels.Attendance;
using Attendance.Web.Models.ViewModels.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Web.Services.Contracts
{
    public interface IPersonService
    {
        Task<IList<PersonViewModel>> GetAllPersonsAsync();
        Task<PersonViewModel> GetPersonByIdAsync(int personId);
        Task<IList<AttendancePersonRowViewModel>> GetPersonsForDayAsync(int dayId);
        Task<int> AddPersonAsync(PersonViewModel person);
        Task UpdatePersonAsync(PersonViewModel person);
    }
}
