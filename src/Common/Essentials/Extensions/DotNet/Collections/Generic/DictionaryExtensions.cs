using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.Essentials.Extensions.DotNet.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dict, 
            TKey key,
            Func<TValue> valueFactory)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            TValue val;
            if (!dict.TryGetValue(key, out val))
            {
                val = valueFactory();
                dict.Add(key, val);
            }
            return val;
        }

        /// <summary>
        /// Returns value for key, or default(TValue) if key does not exist.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue TryGetByKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            TValue val;
            if (dict.TryGetValue(key, out val))
            {
                return val;
            }
            return default(TValue);
        }
    }
}
