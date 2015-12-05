using System;
using System.Collections.Generic;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public class CurrentAttackers
    {
        readonly List<CurrentAttacker> attackers = new List<CurrentAttacker>(); 

        public void Add(string enemyName, LogEntry entry)
        {
            attackers.Add(new CurrentAttacker()
            {
                Name = enemyName,
                AttackedAt = entry.Timestamp
            });
        }

        public IEnumerable<CurrentAttacker> GetCurrentWithCleanup(TimeSpan aliveTreshhold, TimeSpan cleanupTreshhold)
        {
            List<CurrentAttacker> result = new List<CurrentAttacker>();
            DateTime treshValid = DateTime.Now - aliveTreshhold;
            DateTime treshCleanup = DateTime.Now - cleanupTreshhold;
            foreach (var attacker in attackers.ToArray())
            {
                if (attacker.AttackedAt > treshValid)
                    result.Add(attacker);
                if (attacker.AttackedAt < treshCleanup)
                    attackers.Remove(attacker);
            }
            return result;
        }
    }

    public class CurrentAttacker
    {
        public DateTime AttackedAt { get; set; }
        public string Name { get; set; }
    }
}