namespace AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing
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