using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi.Extensions.DotNet.Collections.Generic
{
    static class DictionaryExtensions
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
