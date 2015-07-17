using System;
using System.Runtime.Serialization;

namespace AldurSoft.WurmApi
{
    [Serializable]
    public class WurmApiException : Exception
    {
        public WurmApiException()
        {
        }

        public WurmApiException(string message)
            : base(message)
        {
        }

        public WurmApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected WurmApiException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
