using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3
{
    [Serializable]
    public class WurmAssistantException : Exception
    {
        public WurmAssistantException()
        {
        }

        public WurmAssistantException(string message) : base(message)
        {
        }

        public WurmAssistantException(string message, Exception inner) : base(message, inner)
        {
        }

        protected WurmAssistantException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
