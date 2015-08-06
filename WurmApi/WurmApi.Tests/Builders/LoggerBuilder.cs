using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.Builders
{
    public static class LoggerBuilder
    {
        public static ILogger RedirectToTraceOut(this ILogger logger, bool verbose = false)
        {
            logger.Arrange(
                logger1 => logger1.Log(Arg.IsAny<LogLevel>(), Arg.AnyString, Arg.AnyObject, Arg.IsAny<Exception>())).DoInstead(
                    (LogLevel l, string m, object s, Exception e) =>
                    {
                        Trace.WriteLine(string.Format("Logged: {0}: {1}\r\nat {2} {3}", l, m, s,
                            FormatException(verbose, e)));
                    });
            return logger;
        }

        static string FormatException(bool verbose, Exception e)
        {
            if (e == null) return String.Empty;

            string excStr;
            if (verbose)
            {
                excStr = e.ToString();
            }
            else
            {
                excStr = e.Message;
                var agrExc = e as AggregateException;
                if (agrExc != null)
                {
                    excStr += string.Join(", ", agrExc.InnerExceptions.Select(exception => exception.Message));
                }
                var invExc = e as TargetInvocationException;
                if (invExc != null)
                {
                    excStr += invExc.InnerException.Message;
                }
            }
            return "\r\n Exc: " + excStr;
        }
    }
}
