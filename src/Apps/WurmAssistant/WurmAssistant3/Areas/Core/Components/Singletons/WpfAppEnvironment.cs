using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Components.Singletons
{
    class WpfAppEnvironment : IEnvironment
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
