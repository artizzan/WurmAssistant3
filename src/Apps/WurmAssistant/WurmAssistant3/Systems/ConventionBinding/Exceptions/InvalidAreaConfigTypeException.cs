using System;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class InvalidAreaConfigTypeException : ConventionBindingException
    {
        readonly Type type;

        public InvalidAreaConfigTypeException([NotNull] Type type, Exception innerException)
            : base(innerException)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            this.type = type;
        }

        public override string ToString()
        {
            return
                $"Type {type.FullName} cannot be used as proper {nameof(IAreaConfiguration)}. "
                + "Type should implement this interface, be public and have a parameterless constructor. " 
                + "See inner exception for details. "
                + base.ToString();
        }
    }
}