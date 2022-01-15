using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    public static class GrangerHelpers
    {
        public static class Ages
        {
            public const string
                Young = "Young",
                Adolescent = "Adolescent",
                Mature = "Mature",
                Aged = "Aged",
                Old = "Old",
                Venerable = "Venerable";
        }

        public static class Tags
        {
            public const string
                Starving = "starving",
                Diseased = "diseased",
                Fat = "fat";
        }

        public static readonly TimeSpan BreedingNotInMoodDuration = TimeSpan.FromMinutes(45);

        static readonly IReadOnlyCollection<string> CreatureAges = new List<string>()
        {
            Ages.Young, Ages.Adolescent, Ages.Mature, Ages.Aged, Ages.Old, Ages.Venerable
        };

        public static readonly IReadOnlyCollection<string> CreatureAgesUpcase;

        public static readonly IReadOnlyCollection<string> OtherNamePrefixes = new List<string>()
        {
            Tags.Fat, Tags.Starving, Tags.Diseased
        };

        public static readonly IReadOnlyCollection<string> AllNamePrefixes;

        static GrangerHelpers()
        {
            CreatureAgesUpcase = CreatureAges.Select(x => x.ToUpperInvariant()).ToArray();

            var allnameprefixesBuilder = new List<string>();
            allnameprefixesBuilder.AddRange(CreatureAges);
            allnameprefixesBuilder.AddRange(OtherNamePrefixes);

            AllNamePrefixes = allnameprefixesBuilder.ToArray<string>();
        }

        public static TimeSpan LongestPregnancyPossible { get; } = TimeSpan.FromHours(273);

        public static string RemoveAllPrefixes(string creatureName)
        {
            return new CreatureNameForPrefixRemoval(creatureName).GetNameWithoutPrefixes().Trim();
        }

        class CreatureNameForPrefixRemoval
        {
            // Note: this is interesting early candidate for CreatureName abstraction, consider for WA4

            public string OriginalCreatureName { get; } = string.Empty;
            public bool HasBrandingName { get; } = false;
            public string PartUntilBrandingName { get; } = string.Empty;
            public string PartCoveringBrandingName { get; } = string.Empty;

            const char BrandingNameSeparator = '\'';

            public CreatureNameForPrefixRemoval([NotNull] string creatureName)
            {
                this.OriginalCreatureName = creatureName ?? throw new ArgumentNullException(nameof(creatureName));

                if (creatureName.Length > 0)
                {
                    var substrings = creatureName.Split(BrandingNameSeparator);
                    if (substrings.Length > 1)
                    {
                        HasBrandingName = true;
                        PartUntilBrandingName = substrings[0];
                        PartCoveringBrandingName = string.Join(BrandingNameSeparator.ToString(), substrings.Skip(1));
                    }
                    else
                    {
                        HasBrandingName = false;
                        PartUntilBrandingName = creatureName;
                    }
                }

            }

            public string GetNameWithoutPrefixes()
            {
                var x = PartUntilBrandingName;
                foreach (string prefix in AllNamePrefixes)
                {
                    x = Regex.Replace(x,
                        $@"^{prefix}(\W)|(\W){prefix}(\W)",
                        "$1$2",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                if (HasBrandingName)
                {
                    return string.Join(BrandingNameSeparator.ToString(), x, PartCoveringBrandingName);
                }
                else
                {
                    return x;
                }
            }
        }

        internal static string CapitalizeCreatureName(string lowercasename)
        {
            char firstletter = lowercasename[0];
            firstletter = Char.ToUpperInvariant(firstletter);
            var fixedName = firstletter + lowercasename.Substring(1, lowercasename.Length - 1);
            return fixedName;
        }

        public static CreatureAge ExtractCreatureAge(string prefixedObjectName)
        {
            return CreatureAge.CreateAgeFromRawCreatureNameStartsWith(prefixedObjectName);
        }

        public static bool HasAgeInName(string prefixedObjectName, bool ignoreCase = false)
        {
            foreach (string age in CreatureAges)
            {
                if (ignoreCase)
                {
                    if (prefixedObjectName.Contains(age, StringComparison.OrdinalIgnoreCase)) return true;
                }
                else
                {
                    if (prefixedObjectName.Contains(age)) return true;
                }
            }
            return false;
        }

        public static ExtractParentNamesResult ExtractParentNames(string parentLine, GrangerDebugLogger logger)
        {
            Match motherMatch = ParseMother(parentLine);
            var motherPart = string.Empty;
            var fatherPart = string.Empty;

            if (motherMatch.Success)
            {
                motherPart = motherMatch.Groups["g"].Value;
                motherPart = ExtractCreatureName(motherPart);
                logger.Log("mother set to: " + motherPart);
            }
            Match fatherMatch = ParseFather(parentLine);
            if (fatherMatch.Success)
            {
                fatherPart = fatherMatch.Groups["g"].Value;
                fatherPart = ExtractCreatureName(fatherPart);
                logger.Log("father set to: " + fatherPart);
            }

            return new ExtractParentNamesResult(motherPart, fatherPart);

            Match ParseMother(string line)
            {
                var result = Regex.Match(line,
                    @"(?:mother|Mother) (?:was the|was an|was a|was|is the|is an|is a|is) (?<g>\w+.*?)\.",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return result;
            }

            Match ParseFather(string line)
            {
                var result = Regex.Match(line,
                    @"(?:father|Father) (?:was the|was an|was a|was|is the|is an|is a|is) (?<g>\w+.*?)\.",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return result;
            }
        }

        public static string ExtractCreatureName(string prefixedObjectName)
        {
            return RemoveAllPrefixes(prefixedObjectName);
        }

        public static DateTime? TryParsePregnantUntil(string line, GrangerDebugLogger logger, DateTime now)
        {
            DateTime? pregnantUntil = null;

            if (line.Contains("She will deliver in"))
            {
                logger.Log("found maybe pregnant line");
                Match match = Regex.Match(line, @"She will deliver in about (?<days>\d+)");

                if (match.Success)
                {
                    double length = Double.Parse(match.Groups[1].Value) + 1D;
                    pregnantUntil = now + TimeSpan.FromDays(length);
                    logger.Log("found creature to be pregnant, estimated delivery: " + pregnantUntil);
                }

                logger.Log("finished parsing pregnant line");
            }

            string[] entrySentences = new[]
                {"You are absolutely certain", "You feel confident", "You feel very confident", "You predict"};

            if (entrySentences.Any(line.Contains))
            {
                Match match = Regex.Match(line, $@"(?:{string.Join("|", entrySentences)}) (?:she|he|it) will give birth in (?:(?<days>\d+)(?: days| day))?(?:, |)?(?:(?<hours>\d+)(?: hours| hour))?(?:, |)?(?:(?<minutes>\d+)(?: minutes| minute))?");
                if (match.Success)
                {
                    Double.TryParse(match.Groups["days"].Value, out double days);
                    Double.TryParse(match.Groups["hours"].Value, out double hours);
                    Double.TryParse(match.Groups["minutes"].Value, out double minutes);
                    pregnantUntil = now + TimeSpan.FromDays(days) + TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);
                    logger.Log("found creature to be pregnant, exact delivery: " + pregnantUntil);
                }

                logger.Log("finished parsing pregnant line");
            }

            return pregnantUntil;
        }

        public static string TryParseCreatureNameIfLineContainsDiseased(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Diseased);
        }

        public static string TryParseCreatureNameIfLineContainsFat(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Fat);
        }

        public static string TryParseCreatureNameIfLineContainsStarving(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Starving);
        }

        static string TryParseCreatureNameIfLineContains(string line, string value)
        {
            Match match = Regex.Match(line, value + @" (\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success)
            {
                string possibleCreatureName = match.Groups[1].Value;
                return CapitalizeCreatureName(possibleCreatureName);
            }
            else return null;
        }

        public static CreatureTrait[] ParseTraitsFromLine(string line)
        {
            List<CreatureTrait> result = new List<CreatureTrait>();
            foreach (var trait in CreatureTrait.GetAllPossibleTraits())
            {
                if (line.Contains(trait.ToString()))
                {
                    result.Add(trait);
                }
            }
            return result.ToArray();
        }

        public class ExtractParentNamesResult
        {
            public string Father { get; }
            public string Mother { get; }

            public ExtractParentNamesResult([NotNull] string mother, [NotNull] string father)
            {
                Father = father ?? throw new ArgumentNullException(nameof(father));
                Mother = mother ?? throw new ArgumentNullException(nameof(mother));
            }
        }
    }
}
