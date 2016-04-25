using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Contracts.ActionQueueParsing
{
    public interface IActionQueueParsingCondition
    {
        Guid ConditionId { get; }
        bool Default { get; }
        bool Disabled { get; }
        string Pattern { get; }
        ConditionKind ConditionKind { get; }
        MatchingKind MatchingKind { get; }
    }
}