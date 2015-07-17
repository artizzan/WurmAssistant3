using System;
using System.Runtime.Serialization;

namespace AldurSoft.SimplePersist
{
    [Serializable]
    public class SimplePersistException : Exception
    {
        public SimplePersistException()
        {
        }

        public SimplePersistException(string message)
            : base(message)
        {
        }

        public SimplePersistException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SimplePersistException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}