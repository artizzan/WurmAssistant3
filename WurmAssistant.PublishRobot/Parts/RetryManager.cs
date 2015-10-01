using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant.PublishRobot.Parts
{
    public static class RetryManager
    {
        public static void AutoRetry(Action action, int maxRetries = 3)
        {
            int currentTry = 0;
            List<Exception> exceptions = new List<Exception>();
            while (true)
            {
                try
                {
                    currentTry++;
                    action();
                    break;
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                    if (currentTry >= maxRetries)
                        throw new AggregateException(exceptions);
                }
            }
        }

        public static T AutoRetry<T>(Func<T> func, int maxRetries = 3)
        {
            int currentTry = 0;
            List<Exception> exceptions = new List<Exception>();
            while (true)
            {
                try
                {
                    currentTry++;
                    return func();
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                    if (currentTry >= maxRetries)
                        throw new AggregateException(
                            "Request failed on all retries, see inner exceptions for each failure details",
                            exceptions);
                }
            }
        }
    }
}
