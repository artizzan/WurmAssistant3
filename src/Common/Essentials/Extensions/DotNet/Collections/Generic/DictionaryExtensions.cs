using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            Func<TValue> valueFactory)
        {
            TValue val;
            if (!dict.TryGetValue(key, out val))
            {
                val = valueFactory();
                dict.Add(key, val);
            }
            return val;
        }
    }
}
