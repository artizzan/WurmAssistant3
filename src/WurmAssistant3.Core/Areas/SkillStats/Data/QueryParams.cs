using System;

namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Data
{
    public class QueryParams
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string[] GameCharacters { get; set; }
        public string ServerGroupId { get; set; }
        public QueryKind QueryKind { get; set; }
    }

    public enum QueryKind
    {
        BestSkill,
        TotalSkills
    }
}