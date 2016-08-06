using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Factories
{
    [KernelBind(BindingHint.FactoryProxy), UsedImplicitly]
    public interface ITriggerManagerFactory
    {
        TriggerManager CreateTriggerManager(string characterName);
    }
}
