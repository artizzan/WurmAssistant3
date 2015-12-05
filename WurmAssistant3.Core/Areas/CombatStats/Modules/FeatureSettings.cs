using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    [PersistentObject("CombatStatsFeature_Settings")]
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
