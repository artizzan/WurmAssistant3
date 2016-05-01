using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Eventing
{
    /// <summary>
    /// Marshalls all invocations to a specific thread or group of threads.
    /// </summary>
    public interface IThreadMarshaller
    {
        /// <summary>
        /// Invokes provided action on a specific thread or group of threads.
        /// </summary>
        /// <param name="action"></param>
        void Marshal(Action action);
    }
}
