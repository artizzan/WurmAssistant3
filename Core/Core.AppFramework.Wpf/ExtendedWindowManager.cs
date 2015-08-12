using Caliburn.Micro;

namespace AldursLab.Deprec.Core.AppFramework.Wpf
{
    public interface IExtendedWindowManager : IWindowManager
    {
    }

    public class ExtendedWindowManager : WindowManager, IExtendedWindowManager
    {
    }
}
