namespace AldursLab.WurmAssistant3.Areas.Triggers.Contracts.ActionQueueParsing
{
    public enum ConditionKind
    {
        ActionStart,
        ActionFalstart,
        ActionEnd,
        ActionFalsEnd,
        ActionFalsEndPreviousEvent,
        LevelingStart,
        LevelingEnd
    }
}