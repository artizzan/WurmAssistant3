using System;
using System.Runtime.Serialization;

namespace AldurSoft.SimplePersist
{
    [Serializable]
    public class EntityTypeMismatchException : SimplePersistException
    {
        public EntityTypeMismatchException()
            : base()
        {
        }

        public EntityTypeMismatchException(string message)
            : base(message)
        {
        }

        public EntityTypeMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected EntityTypeMismatchException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}