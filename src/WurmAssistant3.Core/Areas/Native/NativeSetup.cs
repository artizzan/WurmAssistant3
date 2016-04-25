using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Native.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Native.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Native
{
    public static class NativeSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<INativeCalls>().To<Win32NativeCalls>().InSingletonScope();
        }
    }
}
