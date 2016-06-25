using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat
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

        public int Total { get { return counts.Any() ? counts.Sum(pair => pair.Value) : 0; }}
    }

    public class KillStatistics : CountableStrings
    {
    }
}