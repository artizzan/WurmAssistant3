using System;
using System.Runtime.Serialization;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public abstract class ConventionBindingException : Exception
    {
        protected ConventionBindingException()
        {
        }

        protected ConventionBindingException(Exception innerException)
            : base(string.Empty, innerException)
        { }

        protected ConventionBindingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}