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

        public void Restart()
        {
            System.Windows.Forms.Application.Restart();
            // Restart does not automatically shutdown WPF application
            System.Windows.Application.Current.Shutdown();
        }
    }
}
