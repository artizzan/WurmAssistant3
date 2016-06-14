using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelBind(BindingHint.Singleton)]
    class WpfAppEnvironment : IEnvironment
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
