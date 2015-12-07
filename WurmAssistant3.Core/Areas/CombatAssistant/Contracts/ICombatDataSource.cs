using System;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Data.Combat;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Contracts
{
    public interface ICombatDataSource
    {
        CombatStatus CombatStatus { get; }

        event EventHandler<EventArgs> DataChanged;
    }
}