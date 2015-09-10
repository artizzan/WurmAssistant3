using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Model;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.IoC
{
    public static class KernelExtensions
    {
        public static void ProhibitGet<T>(this IKernel kernel)
        {
            kernel.Bind<T>().ToMethod(context =>
            {
                throw new InvalidOperationException(
                    string.Format("Resolving {0} is not allowed. Resolve interfaces implemented by this type.",
                        context.Binding.Service.FullName));
            });
        }

        public static void LogCoreError(this IKernel kernel, Exception exception, string message)
        {
            var loggerFactory = kernel.TryGet<ILoggerFactory>();
            if (loggerFactory != null)
            {
                var logger = loggerFactory.Create("Kernel");
                logger.Error(exception, message);
            }
        }

        public static void LogCoreInfo(this IKernel kernel, string message)
        {
            var loggerFactory = kernel.TryGet<ILoggerFactory>();
            if (loggerFactory != null)
            {
                var logger = loggerFactory.Create("Kernel");
                logger.Info(message);
            }
        }

        public static void LogCoreInfo(this IKernel kernel, Exception exception, string message)
        {
            var loggerFactory = kernel.TryGet<ILoggerFactory>();
            if (loggerFactory != null)
            {
                var logger = loggerFactory.Create("Kernel");
                logger.Info(exception, message);
            }
        }
    }
}
