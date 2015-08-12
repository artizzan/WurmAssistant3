using AldursLab.WurmAssistant3.Attributes;
using AldursLab.WurmAssistant3.Engine.Modules.Calendar.Impl;
using AldursLab.WurmAssistant3.Modules;
using AldursLab.WurmAssistant3.Modules.Calendar;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.Engine.Modules.Calendar
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
