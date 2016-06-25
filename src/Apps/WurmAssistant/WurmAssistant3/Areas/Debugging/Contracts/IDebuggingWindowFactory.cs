using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Debugging.Services;

namespace AldursLab.WurmAssistant3.Areas.Debugging.Contracts
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface IDebuggingWindowFactory
    {
        DebuggingWindow CreateDebuggingWindow();
    }
}
