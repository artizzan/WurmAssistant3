using AldursLab.PersistentObjects;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    [KernelBind(BindingHint.Singleton), PersistentObject("CombatStatsFeature_Settings")]
    public class FeatureSettings : PersistentObjectBase
    {
        byte[] combatResultViewState = new byte[0];

        public byte[] CombatResultViewState
        {
            get { return combatResultViewState; }
            set { combatResultViewState = value; FlagAsChanged(); }
        }
    }
}
