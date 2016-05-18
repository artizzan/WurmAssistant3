using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AldursLab.Essentials.Extensions.DotNet.Collections.Concurrent
{
    public static class ConcurrentQueueExtensions
    {
        public static IEnumerable<T> DequeueAll<T>(this ConcurrentQueue<T> queue)
        {
            List<T> result = new List<T>();

            T item;
            while (queue.TryDequeue(out item))
            {
                result.Add(item);
            }

            return result;
        }
    }
}
