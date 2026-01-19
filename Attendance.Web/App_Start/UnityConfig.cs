using Attendance.Web.Services.Contracts;
using Attendance.Web.Services.Implementations;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace Attendance.Web
{
    public static class UnityConfig
    {
        //installed plugins: Unity(è IoC container per .NET), Unity.Mvc5(x bridge aspnetcore mvc5 - Unity IoC Container)
        //check also file Global.asax.cs
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<IDayService, DayService>();
            container.RegisterType<IPersonService, PersonService>();
            container.RegisterType<IAttendanceService, AttendanceService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}