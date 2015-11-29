namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatActor
    {
        public CombatActor(string name)
        {
            Name = name;
            DamageCausedCounts = new Damage();
            ParryCounts = new Parry();
            TargetPreferenceCounts = new TargetPreference();
            EvadeCounts = new Evasion();
        }

        public string Name { get; private set; }
        public Damage DamageCausedCounts { get; private set; }
        public Parry ParryCounts { get; private set; }
        public TargetPreference TargetPreferenceCounts { get; private set; }
        public Evasion EvadeCounts { get; private set; }
        public int MissesCount { get; set; }
        public int GlancingCount { get; set; }
        public int ShieldBlockCount { get; set; }
    }
}