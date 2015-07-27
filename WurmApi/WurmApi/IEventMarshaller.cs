using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Marshalls event invocations to designated thread.
    /// </summary>
    public interface IEventMarshaller
    {
        /// <summary>
        /// Invokes provided action on a designated thread.
        /// </summary>
        /// <param name="action"></param>
        void Marshal(Action action);
    }
}
