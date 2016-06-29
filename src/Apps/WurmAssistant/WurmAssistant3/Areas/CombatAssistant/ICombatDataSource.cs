using System;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    public interface ICombatDataSource
    {
        CombatStatus CombatStatus { get; }

        event EventHandler<EventArgs> DataChanged;
    }
}