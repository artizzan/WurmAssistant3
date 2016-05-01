using System;
using System.Collections.Generic;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.ActionQueueParsing;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Contracts.ActionQueueParsing
{
    public interface IActionQueueConditions
    {
        event EventHandler<EventArgs> ConditionsChanged;

        IEnumerable<IActionQueueParsingCondition> ActionStart { get; }
        IEnumerable<IActionQueueParsingCondition> ActionFalstart { get; }
        IEnumerable<IActionQueueParsingCondition> ActionEnd { get; }
        IEnumerable<IActionQueueParsingCondition> ActionFalsEnd { get; }
        IEnumerable<IActionQueueParsingCondition> ActionFalsEndPreviousEvent { get; }
        IEnumerable<IActionQueueParsingCondition> LevelingStart { get; }
        IEnumerable<IActionQueueParsingCondition> LevelingEnd { get; }

        void ShowConditionsEditGui();
    }
}