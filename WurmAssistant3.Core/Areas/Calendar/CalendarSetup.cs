using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Calendar
{
    public static class CalendarSetup
    {
        public static void BindCalendar(IKernel kernel)
        {
            kernel.Bind<IFeature>().To<CalendarFeature>().InSingletonScope().Named("Calendar");
            kernel.Bind<WurmSeasonsManager>().ToSelf().InSingletonScope();
        }
    }
}
