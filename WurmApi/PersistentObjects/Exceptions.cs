using System;
using System.Runtime.Serialization;

namespace AldursLab.WurmApi.PersistentObjects
{
    /// <summary>
    /// Indicates that data file, used for persistence, does not exist at specified path.
    /// </summary>
    [Serializable]
    public class PersistenceException : Exception
    {
        public PersistenceException()
        {
        }

        public PersistenceException(string message) : base(message)
        {
        }

        public PersistenceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PersistenceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
