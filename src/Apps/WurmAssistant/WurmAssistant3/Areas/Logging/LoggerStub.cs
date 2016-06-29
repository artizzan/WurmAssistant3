using System;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public class LoggerStub : ILogger
    {
        public void Error(string message)
        {
        }

        public void Error(Exception exception, string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Info(Exception exception, string message)
        {
        }

        public void Warn(string message)
        {
        }

        public void Warn(Exception exception, string message)
        {
        }

        public void Debug(string message)
        {
        }
    }
}