using System;
using System.Collections.ObjectModel;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Infrastructure.Logging;
using NLog;

namespace AldursLab.WurmAssistant3.Core.ViewModels
{
    public class LogOutputViewModel : ILogMessageProcessor
    {
        public ObservableCollection<string> Messages { get; private set; }

        public LogOutputViewModel()
        {
            Messages = new ObservableCollection<string>();
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
            TrimMessages();
        }

        public void Handle(LogLevel level, string message, Exception exception)
        {
            AddMessage(string.Format("{0} > {1}{2}", level, message, FormatException(exception)));
        }

        private string FormatException(Exception exception)
        {
            string result = string.Empty;
            if (exception != null)
            {
                result += " > " + exception.Message;
                if (exception.InnerException != null)
                {
                    result += " > " + exception.InnerException.Message;
                }
            }
            return result;
        }

        void TrimMessages()
        {
            const int maxMessagesInDisplay = 2000;
            const int messageTrimCount = 100;

            if (Messages.Count > maxMessagesInDisplay)
            {
                var oldestMessages = Messages.Take(messageTrimCount).ToArray();
                foreach (var message in oldestMessages)
                {
                    Messages.Remove(message);
                }
            }
        }
    }
}
