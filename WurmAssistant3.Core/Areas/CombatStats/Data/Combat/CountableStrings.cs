using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat
{
    public abstract class CountableStrings
    {
        readonly Dictionary<string, int> counts = new Dictionary<string, int>();

        public void IncrementForName(string name)
        {
            int currentCount = 0;
            counts.TryGetValue(name, out currentCount);
            counts[name] = currentCount + 1;
        }

        public IEnumerable<KeyValuePair<string, int>> Counts { get { return counts; } } 
    }

    public class KillStatistics : CountableStrings
    {
    }
}