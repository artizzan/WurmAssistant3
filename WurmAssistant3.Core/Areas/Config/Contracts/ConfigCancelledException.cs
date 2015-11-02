using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Contracts
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
