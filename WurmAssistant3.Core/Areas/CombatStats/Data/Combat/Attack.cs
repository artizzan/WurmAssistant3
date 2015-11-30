using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class Attack
    {
        public string Type { get; set; }
        public string Strength { get; set; }
        public string Damage { get; set; }
        public string TargetBodyPart { get; set; }
    }

    public class DamageCausedStats
    {
        readonly List<Attack> attacks = new List<Attack>();

        public IEnumerable<Attack> Attacks
        {
            get { return attacks; }
        }

        public void Add(Attack attack)
        {
            attacks.Add(attack);
        }
    }
}