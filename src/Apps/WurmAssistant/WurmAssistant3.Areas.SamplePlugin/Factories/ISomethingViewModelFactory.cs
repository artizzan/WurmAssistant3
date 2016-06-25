using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.SamplePlugin.ViewModels;

namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.Factories
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISomethingViewModelFactory
    {
        SomethingViewModel CreatesSomethingViewModel();
    }
}
