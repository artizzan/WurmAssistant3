using System;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Modules.TriggersManager
{
    public class TriggerException : Exception
    {
        public TriggerException(string message) : base(message) {}
    }
}
