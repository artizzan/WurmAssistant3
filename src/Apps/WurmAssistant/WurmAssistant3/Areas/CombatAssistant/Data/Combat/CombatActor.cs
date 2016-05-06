namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat
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
        /// <summary>
        /// Damage caused to enemy
        /// </summary>
        public DamageCausedStats DamageCausedStats { get; private set; }
        /// <summary>
        /// Parried enemy attacks
        /// </summary>
        public Parry ParryCounts { get; private set; }
        /// <summary>
        /// Targets chosen against enemy
        /// </summary>
        public TargetPreference TargetPreferenceCounts { get; private set; }
        /// <summary>
        /// Evaded enemy attacks
        /// </summary>
        public Evasion EvadedCounts { get; private set; }
        /// <summary>
        /// Misses against the enemy
        /// </summary>
        public int MissesCount { get; set; }
        /// <summary>
        /// Glancing blows against the enemy
        /// </summary>
        public int GlancingBlowsCount { get; set; }
        /// <summary>
        /// Shield blocked attacks of the enemy
        /// </summary>
        public int ShieldBlockCount { get; set; }

        // number of spell hits caused to other actor
        public int FreezingHits { get; set; }
        public int AffectingHits { get; set; }
        public int LifeLeeched { get; set; }
        public int FlamingHits { get; set; }

        /// <summary>
        /// Number of creatures slain by Character
        /// </summary>
        public int SlainCount { get; set; }

        /// <summary>
        /// Fighting skill gained from kills of this actor
        /// </summary>
        public float FightingSkillGained { get; set; }
    }
}