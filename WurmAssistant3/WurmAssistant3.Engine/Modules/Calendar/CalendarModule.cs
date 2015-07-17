using System;

using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Attributes;
using AldurSoft.WurmAssistant3.Engine.Modules.Calendar.Impl;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Modules.Calendar;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules.Calendar
{
    [WurmAssistantModule("Calendar")]
    public class CalendarModule : ModuleBase, ICalendarModule
    {
        private readonly CalendarDataContext dataContext;

        public CalendarModule(
            ModuleId moduleId,
            IModuleEngine moduleEngine,
            IPersistentManager persistentManager)
            : base(moduleId, moduleEngine, persistentManager)
        {
            this.dataContext = new CalendarDataContext(persistentManager);
        }
    }
}
