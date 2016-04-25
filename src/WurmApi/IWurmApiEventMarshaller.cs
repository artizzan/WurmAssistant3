using System;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Marshalls public WurmApi event invocations to designated thread.
    /// </summary>
    public interface IWurmApiEventMarshaller
    {
        /// <summary>
        /// Invokes provided action on a designated thread.
        /// </summary>
        /// <param name="action"></param>
        void Marshal(Action action);
    }
}
