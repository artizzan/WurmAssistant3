using AldursLab.WurmAssistant3.Areas.Native.Contracts;
using AldursLab.WurmAssistant3.Areas.Native.Modules;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Native
{
    public static class NativeSetup
    {
        public static void Bind(IKernel kernel)
        {
            kernel.Bind<INativeCalls>().To<Win32NativeCalls>().InSingletonScope();
        }
    }
}
