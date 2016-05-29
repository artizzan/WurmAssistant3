using System;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions
{
    [Serializable]
    public class InvalidAreaConfigTypeException : ConventionBindingException
    {
        readonly AreaTypeReflectionInfo areaType;

        public InvalidAreaConfigTypeException([NotNull] AreaTypeReflectionInfo areaType, Exception innerException)
            : base(innerException)
        {
            if (areaType == null) throw new ArgumentNullException(nameof(areaType));
            this.areaType = areaType;
        }

        public override string ToString()
        {
            return
                $"Type {areaType.Type.FullName} cannot be used as proper {nameof(IAreaConfiguration)}. "
                + "Type should implement this interface, be public and have a parameterless constructor. " 
                + "See inner exception for details. "
                + base.ToString();
        }
    }
}