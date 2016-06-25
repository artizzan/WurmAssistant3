using System;
using System.Drawing;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface ISystemTrayContextMenu
    {
        void AddMenuItem(string text, Action onClick, Image image);
        event EventHandler<EventArgs> ExitWurmAssistantClicked;
        event EventHandler<EventArgs> ShowMainWindowClicked;
    }
}
