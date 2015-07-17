using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.Core
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
