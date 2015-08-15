using Caliburn.Micro;

namespace AldursLab.WurmAssistant3.ViewModels
{
    public class AppHostViewModel : Screen
    {
        Screen currentScreen;

        public Screen CurrentScreen
        {
            get { return currentScreen; }
            set
            {
                if (Equals(value, currentScreen)) return;
                currentScreen = value;
                NotifyOfPropertyChange(() => CurrentScreen);
            }
        }
    }
}
