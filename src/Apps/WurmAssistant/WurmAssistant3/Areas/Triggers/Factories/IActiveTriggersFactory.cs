using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Factories
{
    [KernelBind(BindingHint.FactoryProxy), UsedImplicitly]
    public interface IActiveTriggersFactory
    {
        ActiveTriggers CreateActiveTriggers(string characterName);
    }
}