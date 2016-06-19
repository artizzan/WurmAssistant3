using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Main.ViewModels;

namespace AldursLab.WurmAssistant3.Areas.Main.Contracts
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface INewsViewModelFactory
    {
        NewsViewModel CreateNewsViewModel();
    }
}
