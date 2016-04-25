using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Extensions.DotNet.Collections.Generic
{
    static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static TSource FirstOrValue<TSource>(this IEnumerable<TSource> source, Func<TSource> valueFactory)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                if (list.Count > 0)
                    return list[0];
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }
            return valueFactory();
        }

        /// <summary>
        /// Applies the action to each element in the list.
        /// </summary>
        /// <typeparam name="T">The enumerable item's type.</typeparam>
        /// <param name="enumerable">The elements to enumerate.</param>
        /// <param name="action">The action to apply to each item in the list.</param>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static TimeSpan Average(this IEnumerable<TimeSpan> enumerable)
        {
            return new TimeSpan((long)enumerable.Select(span => span.Ticks).Average());
        }
    }
}
