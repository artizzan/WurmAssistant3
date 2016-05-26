using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public sealed class AreaReflectionInfo : IEquatable<AreaReflectionInfo>
    {
        public AreaReflectionInfo([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var areaNameMatch = TryParseAreaNameFromType(type);
            AreaName = areaNameMatch ?? string.Empty;
            AreaAssembly = type.Assembly;
        }
        
        public string AreaName { get; }
        public Assembly AreaAssembly { get; }
        public bool AreaKnown => !string.IsNullOrEmpty(AreaName);

        public static string TryParseAreaNameFromType([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.Namespace == null) throw new ArgumentException($"Namespace of type {type.FullName} is null");

            var areaNameMatch = Regex.Match(type.Namespace,
                @"AldursLab\.WurmAssistant3\.Areas\.([^\.]+)(\..+)?");

            return areaNameMatch.Success ? areaNameMatch.Groups[1].Value : null;
        }

        public static string ParseAreaNameFromType([NotNull] Type type)
        {
            var result = TryParseAreaNameFromType(type);
            if (result == null)
            {
                throw new InvalidOperationException($"Unable to parse area name from type {type.FullName}");
            }
            return result;
        }

        public IEnumerable<Type> GetAllAreaTypes()
        {
            return AreaAssembly.GetTypes()
                               .Where(
                                   type =>
                                       type.Namespace != null
                                       && type.Namespace.StartsWith($"AldursLab.WurmAssistant3.Areas.{AreaName}"));
        }

        #region IEquatable

        public bool Equals(AreaReflectionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(AreaName, other.AreaName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AreaReflectionInfo) obj);
        }

        public override int GetHashCode()
        {
            return AreaName.GetHashCode();
        }

        public static bool operator ==(AreaReflectionInfo left, AreaReflectionInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AreaReflectionInfo left, AreaReflectionInfo right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}