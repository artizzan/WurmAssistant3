using System;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class InvalidAreaConfigTypeException : ConventionBindingException
    {
        readonly WaTypeInfo areaWaType;

        public InvalidAreaConfigTypeException([NotNull] WaTypeInfo areaWaType, Exception innerException)
            : base(innerException)
        {
            if (areaWaType == null) throw new ArgumentNullException(nameof(areaWaType));
            this.areaWaType = areaWaType;
        }

        public override string ToString()
        {
            return
                $"Type {areaWaType.Type.FullName} cannot be used as proper {nameof(AreaConfig)}. "
                + "Type should implement this abstract class, be public and have a parameterless constructor. " 
                + "See inner exception for details. "
                + base.ToString();
        }
    }
}