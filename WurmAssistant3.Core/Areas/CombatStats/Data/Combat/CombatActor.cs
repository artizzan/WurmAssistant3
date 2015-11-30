namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatActor
    {
        public CombatActor(string name)
        {
            Name = name;
            DamageCausedStats = new DamageCausedStats();
            ParryCounts = new Parry();
            TargetPreferenceCounts = new TargetPreference();
            EvadedCounts = new Evasion();
        }

        public string Name { get; private set; }
        public DamageCausedStats DamageCausedStats { get; private set; }
        public Parry ParryCounts { get; private set; }
        public TargetPreference TargetPreferenceCounts { get; private set; }
        public Evasion EvadedCounts { get; private set; }
        public int MissesCount { get; set; }
        public int GlancingReceivedCount { get; set; }
        public int ShieldBlockCount { get; set; }
    }
}