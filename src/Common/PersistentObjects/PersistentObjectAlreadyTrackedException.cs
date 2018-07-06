using System;
using System.Runtime.Serialization;

namespace AldursLab.PersistentObjects
{
    [Serializable]
    public class PersistentObjectAlreadyTrackedException : Exception
    {
        public PersistentObjectAlreadyTrackedException()
        {
        }

        public PersistentObjectAlreadyTrackedException(string message)
            : base(message)
        {
        }

        public PersistentObjectAlreadyTrackedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected PersistentObjectAlreadyTrackedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}