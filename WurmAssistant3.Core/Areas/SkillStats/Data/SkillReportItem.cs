namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Data
{
    public class SkillReportItem
    {
        public string Name { get; set; }
        public float CurrentValue { get; set; }
    }

    public class SkillGainReportItem : SkillReportItem
    {
        public string GameCharacter { get; set; }
        public float StartValue { get; set; }
        public float Gain { get { return CurrentValue - StartValue; }}
    }

    public class SkillLevelReportItem : SkillReportItem
    {
        public string GameCharacter { get; set; }
    }

    public class LiveSkillReportItem : SkillReportItem
    {
        public float StartValue { get; set; }
        public double AverageGainPerHour { get; set; }
        public float TotalGain { get { return CurrentValue - StartValue; } }
    }
}
