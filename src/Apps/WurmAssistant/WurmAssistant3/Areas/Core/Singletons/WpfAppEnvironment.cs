using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Singletons
{
    class WpfAppEnvironment : IEnvironment
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
