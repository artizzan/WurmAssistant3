using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.TrayPopups
{
    public static class TrayPopupsSetup
    {
        public static void BindTrayPopups(IKernel kernel)
        {
            kernel.Bind<ITrayPopups>().To<Modules.TrayPopups>().InSingletonScope();
        }
    }
}
