namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    static class WurmSkills
    {
        public static string NormalizeSkillName(string skillName)
        {
            return skillName.ToUpperInvariant();
        }
    }
}