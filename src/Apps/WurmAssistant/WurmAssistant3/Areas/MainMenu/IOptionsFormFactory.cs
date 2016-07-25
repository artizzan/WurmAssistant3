using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IOptionsFormFactory
    {
        OptionsForm CreateOptionsForm();
    }
}
