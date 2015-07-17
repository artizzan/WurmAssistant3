using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.SimplePersist
{
    public class SerializationErrors
    {
        private readonly IEnumerable<Exception> exceptions;

        public SerializationErrors()
        {
            this.exceptions = new Exception[0];
        }

        public SerializationErrors(IEnumerable<Exception> exceptions)
        {
            if (exceptions == null) throw new ArgumentNullException("exceptions");
            this.exceptions = exceptions;
        }

        public IEnumerable<Exception> Exceptions
        {
            get { return exceptions; }
        }

        public override string ToString()
        {
            return string.Join("\r\n", exceptions.Select(exception => exception.ToString()));
        }
    }
}