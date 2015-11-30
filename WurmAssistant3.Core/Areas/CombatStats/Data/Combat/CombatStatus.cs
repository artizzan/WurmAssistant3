using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CombatStatus
    {
        readonly string characterName;

        readonly Dictionary<Tuple<string, string>,CombatActorPairStats> combatStatsMap 
            = new Dictionary<Tuple<string, string>, CombatActorPairStats>();

        public CombatStatus(string characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            this.characterName = characterName;
        }

        public CombatActorPairStats GetStatsFor(string actorOne, string actorTwo)
        {
            CombatActorPairStats stats;

            // normalizing ordering of the keys, to ensure 
            // that a reversed combination of actors matches same map entry, as non reversed.
            var keys = new[] {actorOne, actorTwo}.OrderBy(s => s).ToArray();
            var key = new Tuple<string, string>(keys[0], keys[1]);

            if (!combatStatsMap.TryGetValue(key, out stats))
            {
                stats = new CombatActorPairStats(new CombatActor(key.Item1), new CombatActor(key.Item2));
                combatStatsMap[key] = stats;
            }
            return stats;
        }

        public IEnumerable<CombatActorPairStats> AllStats { get { return combatStatsMap.Values; } }

        public string CharacterName
        {
            get { return characterName; }
        }

        public void EnemyBeginsAttack(string enemyName)
        {
            throw new NotImplementedException();
        }

        public Focus CurrentFocus { get; private set; }
    }
}
