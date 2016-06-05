using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Config.Singletons;
using AldursLab.WurmAssistant3.Areas.Config.Transients;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Areas.Config
{
    public static class ConfigSetup
    {
        public static void BindComponents(IKernel kernel)
        {
            kernel.Bind<ISettingsEditViewFactory>().ToFactory();
            kernel.Bind<SettingsEditForm>().ToSelf();

            kernel.Bind<IServersEditorViewFactory>().ToFactory();
            kernel.Bind<ServersEditorForm>().ToSelf();
            kernel.Bind<ServerInfoManager>().ToSelf().InSingletonScope();
        }
    }
}
