using Caliburn.Micro;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.ViewModels
{
    public class AppStartViewModel : Screen
    {
        LogOutputViewModel logOutputViewModel;

        [Inject]
        public LogOutputViewModel LogOutputViewModel
        {
            get { return logOutputViewModel; }
            set
            {
                if (Equals(value, logOutputViewModel)) return;
                logOutputViewModel = value;
                NotifyOfPropertyChange(() => LogOutputViewModel);
            }
        }
    }
}
