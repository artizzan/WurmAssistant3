using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public class AreaTypesManager
    {
        readonly AreaReflectionInfo areaReflectionInfo;
        readonly Type[] types;

        public AreaTypesManager([NotNull] AreaReflectionInfo areaReflectionInfo)
        {
            if (areaReflectionInfo == null) throw new ArgumentNullException(nameof(areaReflectionInfo));
            this.areaReflectionInfo = areaReflectionInfo;

            types = areaReflectionInfo.GetAllAreaTypes().ToArray();
        }

        public string AreaName => areaReflectionInfo.AreaName;

        public IEnumerable<Type> GetAllSingletons() { return FilterComponents("Singletons"); }
        public IEnumerable<Type> GetAllTransients() { return FilterComponents("Transients"); }
        public IEnumerable<Type> GetAllAreaScoped() { return FilterComponents("AreaScoped"); }
        public IEnumerable<Type> GetAllViewModels() { return FilterComponents("ViewModels"); }
        public IEnumerable<Type> GetAllCustomViews() { return FilterComponents("CustomViews"); }
        public IEnumerable<Type> GetAllAutoFactories() { return FilterComponents("AutoFactories"); }

        IEnumerable<Type> FilterComponents(string componentType)
        {
            return types.Where(
                // ReSharper disable once PossibleNullReferenceException
                type => type.Namespace.StartsWith(FormatForComponentType(componentType)));
        }

        string FormatForComponentType(string componentType)
        {
            return $@"AldursLab\.WurmAssistant3\.Areas\.{AreaName}\.{componentType}";
        }
    }
}