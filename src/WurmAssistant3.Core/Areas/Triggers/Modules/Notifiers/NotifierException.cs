using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers
{
    public class NotifierException : Exception
    {
        public NotifierException(string message) : base(message) {}
    }
}
