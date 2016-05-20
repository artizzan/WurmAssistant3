using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Areas.Granger.Modules.LogFeedManager
{
    public static class GrangerHelpers
    {
        public static TimeSpan Breeding_NotInMood_Duration = TimeSpan.FromMinutes(45);

        public const string
            YOUNG = "Young",
            ADOLESCENT = "Adolescent",
            MATURE = "Mature",
            AGED = "Aged",
            OLD = "Old",
            VENERABLE = "Venerable";

        public const string
            STARVING = "starving",
            DISEASED = "diseased",
            FAT = "fat";

        public static string[] CreatureAges = 
        {
            YOUNG, ADOLESCENT, MATURE, AGED, OLD, VENERABLE
        };

        public static string[] CreatureAgesUpcase;

        static string[] WildCreatureNames = 
        {
            //todo: this is no longer useful, remove functionality
            "Horse", "Hell horse", "deer", "cow", "bull", "bison", 
        };

        public static string[] OtherNamePrefixes =
        {
            FAT, STARVING, DISEASED
        };

        public static string[] AllNamePrefixes;

        static GrangerHelpers()
        {
            CreatureAgesUpcase = CreatureAges.Select(x => x.ToUpperInvariant()).ToArray();

            var allnameprefixesBuilder = new List<string>();
            allnameprefixesBuilder.AddRange(CreatureAges);
            allnameprefixesBuilder.AddRange(OtherNamePrefixes);

            //// Fix for recent change in casing creature ages, todo refactor
            //allnameprefixesBuilder.AddRange(CreatureAges.Select(s => s.ToLowerInvariant()));

            AllNamePrefixes = allnameprefixesBuilder.ToArray<string>();
        }

        public static string RemoveAllPrefixes(string creatureName)
        {
            foreach (string prefix in AllNamePrefixes)
            {
                //
                creatureName = Regex.Replace(creatureName,
                    $@"^{prefix}(\W)|(\W){prefix}(\W)",
                    "$1$2",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            creatureName = creatureName.Trim();
            return creatureName;
        }

        /// <summary>
        /// capitalizes lower-case creature name
        /// </summary>
        /// <param name="lowercasename"></param>
        /// <returns></returns>
        internal static string FixCase(string lowercasename)
        {
            char firstletter = lowercasename[0];
            firstletter = Char.ToUpperInvariant(firstletter);
            var fixedName = firstletter + lowercasename.Substring(1, lowercasename.Length - 1);
            return fixedName;
        }

        static TimeSpan _LongestPregnancyPossible = TimeSpan.FromHours(273);
        public static TimeSpan LongestPregnancyPossible { get { return _LongestPregnancyPossible; } }

        public static CreatureAge ExtractCreatureAge(string prefixedObjectName)
        {
            return CreatureAge.CreateAgeFromRawCreatureNameStartsWith(prefixedObjectName);
        }

        /// <summary>
        /// Checks if provided string is EQUAL to blacklisted (wild) creature name.
        /// </summary>
        /// <param name="fixedCreatureName"></param>
        /// <returns></returns>
        public static bool IsBlacklistedCreatureName_EqualCheck(string fixedCreatureName)
        {
            fixedCreatureName = fixedCreatureName.ToUpperInvariant();
            foreach (string name in WildCreatureNames)
            {
                if (fixedCreatureName == name.ToUpperInvariant()) return true;
            }
            return false;
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

        public static string ExtractCreatureName(string prefixedObjectName)
        {
            return RemoveAllPrefixes(prefixedObjectName);
        }

        /// <summary>
        /// Returns possible creature name if line contains DISEASED. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsDiseased(string inputLine)
        {
            // try to match for [age] diseased [creaturename]
            return LineContains(inputLine, DISEASED);
        }

        /// <summary>
        /// Returns possible creature name if line contains FAT. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsFat(string inputLine)
        {
            // try to match for [age] diseased [creaturename]
            return LineContains(inputLine, FAT);
        }

        /// <summary>
        /// Returns possible creature name if line contains STARVING. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsStarving(string inputLine)
        {
            // try to match for [age] diseased [creaturename]
            return LineContains(inputLine, STARVING);
        }

        static string LineContains(string input, string value)
        {
            Match match = Regex.Match(input, value + @" (\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success)
            {
                string possibleCreatureName = match.Groups[1].Value;
                return FixCase(possibleCreatureName);
            }
            else return null;
        }

        public static CreatureTrait[] GetTraitsFromLine(string line)
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
    }
}
