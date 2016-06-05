using System;
using System.Text.RegularExpressions;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    public sealed class AreaTypeReflectionInfo
    {
        public string AreaName { get; }
        public bool AreaKnown => !string.IsNullOrEmpty(AreaName);
        public Type Type { get; }
        public bool IsAreaConfiguration { get; }

        public AreaTypeReflectionInfo([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            AreaName = type.TryParseAreaName() ?? string.Empty;
            IsAreaConfiguration = type.IsAreaConfigurationType();
            Type = type;
        }

        public IAreaConfiguration ActivateAsAreaConfiguration()
        {
            try
            {
                return (IAreaConfiguration)Activator.CreateInstance(Type);
            }
            catch (Exception exception)
            {
                throw new InvalidAreaConfigTypeException(this, exception);
            }
        }

        public bool MatchesAreaName(string areaName)
        {
            return AreaName.ToUpperInvariant() == areaName.ToUpperInvariant();
        }

        public bool MatchesComponentType(string componentType)
        {
            return Type.IsAreaComponentType(AreaName, componentType);
        }
    }
}