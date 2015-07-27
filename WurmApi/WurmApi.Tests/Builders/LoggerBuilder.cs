using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    (LogLevel l, string m, string s, Exception e) =>
                    {
                        Trace.WriteLine(string.Format("Logged: {0}: {1}\r\nat {2} {3}", l, m, s,
                            e != null ? "\r\n Exc: " + (verbose ? e.ToString() : e.Message) : string.Empty));
                    });
            return logger;
        }
    }
}
