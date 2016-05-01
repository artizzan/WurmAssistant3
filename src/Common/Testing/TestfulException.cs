using System;
using System.Runtime.Serialization;

namespace AldursLab.Testing
{
    [Serializable]
    public class TestfulException : Exception
    {
        public TestfulException()
        {
        }

        public TestfulException(string message) : base(message)
        {
        }

        public TestfulException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestfulException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}