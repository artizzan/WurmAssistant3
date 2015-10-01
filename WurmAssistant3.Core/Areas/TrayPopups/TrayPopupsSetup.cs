using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.TrayPopups
{
    public static class TrayPopupsSetup
    {
        public static void BindTrayPopups(IKernel kernel)
        {
            kernel.Bind<ITrayPopups>().To<Modules.TrayPopups>().InSingletonScope();
        }
    }
}
