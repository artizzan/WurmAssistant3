using System;
using System.Runtime.Serialization;

namespace AldurSoft.WurmApi
{
    [Serializable]
    public class ThreadGuardException : Exception
    {
        public ThreadGuardException()
        {
        }

        public ThreadGuardException(string message)
            : base(message)
        {
        }

        public ThreadGuardException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ThreadGuardException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}