using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    [KernelBind(BindingHint.Singleton)]
    public class AppMigrationsManager : IAppManager
    {
        public AppMigrationsManager()
        {
        }

        public void RunMigrations()
        {
            
        }
    }
}
