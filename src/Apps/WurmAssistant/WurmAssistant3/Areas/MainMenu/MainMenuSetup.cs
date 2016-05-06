using AldursLab.WurmAssistant3.Areas.MainMenu.Views;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    public class MainMenuSetup
    {
        public static void BindMenu(IKernel kernel)
        {
            kernel.Bind<MenuView>().ToSelf().InSingletonScope();
        }
    }
}
