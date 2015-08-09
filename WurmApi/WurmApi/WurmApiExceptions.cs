using System;
using System.Collections.Generic;
using System.Linq;
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

    [Serializable]
    public class DataNotFoundException : WurmApiException
    {
        public DataNotFoundException() 
            : base("Requested data has not been found or does not yet exist.")
        {
        }

        public static DataNotFoundException CreateFromKeys(params object[] keys)
        {
            return new DataNotFoundException(keys);
        }

        public static DataNotFoundException CreateFromKeysWithInner(Exception inner, params object[] keys)
        {
            return new DataNotFoundException(inner, keys);
        }

        public DataNotFoundException(string message)
            : base(message)
        {
        }

        public DataNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private DataNotFoundException(object[] keys)
            : base(string.Format("Requested data has not been found or does not yet exist. Provided keys: {0}", string.Join(", ", keys)))
        {
        }

        private DataNotFoundException(Exception inner, object[] keys)
            : base(string.Format("Requested data has not been found or does not yet exist. Provided keys: {0}", string.Join(", ", keys)), inner)
        {
        }

        protected DataNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class WurmGameClientInstallDirectoryValidationException : Exception
    {
        public WurmGameClientInstallDirectoryValidationException()
        {
        }

        public WurmGameClientInstallDirectoryValidationException(string message)
            : base(message)
        {
        }

        public WurmGameClientInstallDirectoryValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected WurmGameClientInstallDirectoryValidationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class HttpWebRequestFailedException : Exception
    {
        /// <summary>
        /// Errors before final retry attempt, if any.
        /// </summary>
        public IEnumerable<Exception> PreviousErrors { get; private set; }

        public HttpWebRequestFailedException()
        {
            PreviousErrors = new Exception[0];
        }

        public HttpWebRequestFailedException(string message)
            : base(message)
        {
            PreviousErrors = new Exception[0];
        }

        public HttpWebRequestFailedException(string message, Exception inner, IEnumerable<Exception> previousExceptions)
            : base(message, inner)
        {
            PreviousErrors = previousExceptions != null ? previousExceptions.ToArray() : new Exception[0];
        }

        protected HttpWebRequestFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
