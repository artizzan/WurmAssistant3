using System;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Logging.Modules
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