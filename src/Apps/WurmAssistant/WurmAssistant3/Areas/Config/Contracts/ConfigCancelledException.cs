using System;
using System.Runtime.Serialization;

namespace AldursLab.WurmAssistant3.Areas.Config.Contracts
{
    [Serializable]
    public class ConfigCancelledException : Exception
    {
        public ConfigCancelledException()
        {
        }

        public ConfigCancelledException(string message) : base(message)
        {
        }

        public ConfigCancelledException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConfigCancelledException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
