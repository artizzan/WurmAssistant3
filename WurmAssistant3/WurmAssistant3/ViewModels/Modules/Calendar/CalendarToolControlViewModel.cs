using System;

using AldurSoft.WurmAssistant3.Modules.Calendar;

using Core.AppFramework.Wpf.ViewModels;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.ViewModels.Modules.Calendar
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
