using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Core.Areas.MainMenu.Views;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Views;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Core.Areas.MainMenu
{
    public class MainMenuSetup
    {
        public static void BindMenu(IKernel kernel)
        {
            kernel.Bind<MenuView>().ToSelf().InSingletonScope();
        }
    }
}
