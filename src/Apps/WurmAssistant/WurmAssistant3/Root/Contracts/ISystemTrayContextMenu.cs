using System;
using System.Drawing;

namespace AldursLab.WurmAssistant3.Root.Contracts
{
    public interface ISystemTrayContextMenu
    {
        void AddMenuItem(string text, Action onClick, Image image);
    }
}
