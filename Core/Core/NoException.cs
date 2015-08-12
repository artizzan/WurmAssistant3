using System;
using System.Runtime.Serialization;

namespace AldursLab.Deprec.Core
{
    [Serializable]
    public class NoException : Exception
    {
        public static readonly NoException Instance = new NoException();

        private NoException()
            : base("No exception has happened. This is a null-object.")
        {
        }

        protected NoException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
