using System;
using System.Runtime.Serialization;

namespace AldursLab.PersistentObjects
{
    [Serializable]
    public class PersistentObjectAlreadyTracked : Exception
    {
        public PersistentObjectAlreadyTracked()
        {
        }

        public PersistentObjectAlreadyTracked(string message)
            : base(message)
        {
        }

        public PersistentObjectAlreadyTracked(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected PersistentObjectAlreadyTracked(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}