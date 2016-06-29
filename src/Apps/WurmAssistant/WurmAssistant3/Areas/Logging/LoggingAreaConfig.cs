using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    [UsedImplicitly]
    public class LoggingAreaConfig : AreaConfig
    {
        public override void Configure(IKernel kernel)
        {
            kernel.Bind<ILogger>().ToMethod(context =>
            {
                // create logger with category matching target type name
                var factory = context.Kernel.Get<ILoggerFactory>();
                if (context.Request.Target != null)
                {
                    var type = context.Request.Target.Member.DeclaringType;
                    return factory.Create(type != null ? type.FullName : string.Empty);
                }
                else
                {
                    return factory.Create("");
                }
            });
        }
    }
}
