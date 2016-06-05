using System;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.Notifiers
{
    public class NotifierException : Exception
    {
        public NotifierException(string message) : base(message) {}
    }
}
