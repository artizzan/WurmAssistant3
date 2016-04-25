using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Config.Views;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Contracts
{
    public interface ISettingsEditViewFactory
    {
        SettingsEditView CreateSettingsEditView();
    }
}
