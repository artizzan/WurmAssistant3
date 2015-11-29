using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatResults
    {
        readonly Dictionary<Tuple<string, string>,CombatStats> combatStatsMap 
            = new Dictionary<Tuple<string, string>, CombatStats>();

        public CombatStats GetStatsFor(string actorOne, string actorTwo)
        {
            CombatStats stats;

            // normalizing ordering of the keys, to ensure 
            // that a reversed combination of actors matches same map entry, as non reversed.
            var keys = new[] {actorOne, actorTwo}.OrderBy(s => s).ToArray();
            var key = new Tuple<string, string>(keys[0], keys[1]);

            if (!combatStatsMap.TryGetValue(key, out stats))
            {
                stats = new CombatStats(new CombatActor(key.Item1), new CombatActor(key.Item2));
                combatStatsMap[key] = stats;
            }
            return stats;
        }

        public IEnumerable<CombatStats> AllStats { get { return combatStatsMap.Values; } } 
    }
}
