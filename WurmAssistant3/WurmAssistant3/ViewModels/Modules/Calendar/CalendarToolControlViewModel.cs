using System;
using AldursLab.Deprec.Core;
using AldursLab.WurmAssistant3.Modules.Calendar;

namespace AldursLab.WurmAssistant3.ViewModels.Modules.Calendar
{
    public class CalendarToolControlViewModel : ModuleToolControlViewModel
    {
        private readonly ICalendarModule calendarModule;

        public CalendarToolControlViewModel([NotNull] ICalendarModule calendarModule)
        {
            if (calendarModule == null) throw new ArgumentNullException("calendarModule");
            this.calendarModule = calendarModule;
        }
    }
}
