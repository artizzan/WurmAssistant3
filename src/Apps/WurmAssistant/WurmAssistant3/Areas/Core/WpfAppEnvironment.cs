using System.Windows;

namespace AldursLab.WurmAssistant3.Areas.Core
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
