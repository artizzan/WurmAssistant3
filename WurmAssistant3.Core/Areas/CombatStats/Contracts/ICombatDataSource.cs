using System;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts
{
    public interface ICombatDataSource
    {
        CombatStatus CombatStatus { get; }

        event EventHandler<EventArgs> DataChanged;
    }
}