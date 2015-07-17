using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

namespace Core.AppFramework.Wpf
{
    public interface IExtendedWindowManager : IWindowManager
    {
    }

    public class ExtendedWindowManager : WindowManager, IExtendedWindowManager
    {
    }
}
