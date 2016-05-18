using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet.Collections.Generic
{
    public static class QueueExtensions
    {
        public static IEnumerable<T> DequeueAll<T>(this Queue<T> queue)
        {
            List<T> result = new List<T>();

            var itemCount = queue.Count;
            for (int i = 0; i < itemCount; i++)
            {
                result.Add(queue.Dequeue());
            }
            return result;
        }
    }
}
