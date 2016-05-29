using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    public static class AreaHelpers
    {
        public static string TryParseAreaName([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.Namespace == null) throw new ArgumentException($"Namespace of type {type.FullName} is null");

            var areaNameMatch = Regex.Match(type.Namespace,
                @"^AldursLab\.WurmAssistant3\.Areas\.([^\.]+)(\..+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return areaNameMatch.Success ? areaNameMatch.Groups[1].Value : null;
        }

        public static string ParseAreaName([NotNull] this Type type)
        {
            var result = type.TryParseAreaName();
            if (result == null)
            {
                throw new InvalidOperationException($"Unable to parse area name from type {type.FullName}");
            }
            return result;
        }

        public static bool IsAreaScopedType(this Type type)
        {
            return type.TryParseAreaName() == null;
        }

        public static bool IsAreaConfigurationType(this Type type)
        {
            var validNamespace = type.Namespace != null;
            var notTheInterface = type != typeof(IAreaConfiguration) && !type.IsInterface;
            var isAreaConfig = typeof(IAreaConfiguration).IsAssignableFrom(type);
            return validNamespace && notTheInterface && isAreaConfig;
        }

        public static bool IsAreaComponentType(this Type type, string areaName, string componentType)
        {
            return Regex.IsMatch(type.Namespace ?? string.Empty,
                $@"^AldursLab\.WurmAssistant3\.Areas\.{areaName}\.{componentType}(?:[\.]|$)",
                RegexOptions.IgnoreCase);
        }
    }
}