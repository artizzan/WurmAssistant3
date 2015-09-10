using System;
using System.Collections.ObjectModel;
using System.Linq;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Model;
using JetBrains.Annotations;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.ViewModels
{
    public class LogViewModel
    {
        readonly ILogMessagePublisher logMessagePublisher;
        readonly IEventMarshaller eventMarshaller;
        public ObservableCollection<string> Messages { get; private set; }

        public LogViewModel([NotNull] ILogMessagePublisher logMessagePublisher,
            [NotNull] IEventMarshaller eventMarshaller)
        {
            if (logMessagePublisher == null) throw new ArgumentNullException("logMessagePublisher");
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            this.logMessagePublisher = logMessagePublisher;
            this.eventMarshaller = eventMarshaller;
            Messages = new ObservableCollection<string>();

            logMessagePublisher.EventLogged += LogMessagePublisherOnEventLogged;
        }

        void LogMessagePublisherOnEventLogged(object sender, LogMessageEventArgs logMessageEventArgs)
        {
            eventMarshaller.Marshal(() =>
            {
                Handle(logMessageEventArgs.Level, logMessageEventArgs.Message, logMessageEventArgs.Exception);
            });
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
            TrimMessages();
        }

        public void Handle(LogLevel level, string message, Exception exception)
        {
            AddMessage(string.Format("{0} > {1}{2}", level, message ?? "NULL", FormatException(exception)));
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
