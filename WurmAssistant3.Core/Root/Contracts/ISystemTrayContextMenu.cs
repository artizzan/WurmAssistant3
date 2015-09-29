using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Root.Contracts
{
    public interface ISystemTrayContextMenu
    {
        void AddMenuItem(string text, Action onClick, Image image);
    }
}
