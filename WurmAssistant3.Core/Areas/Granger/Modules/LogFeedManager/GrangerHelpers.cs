using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager
{
    public static class GrangerHelpers
    {
        public static TimeSpan Breeding_NotInMood_Duration = TimeSpan.FromMinutes(45);

        //DRY matching maps for wurm log text, in case it ever changes
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

        public static string[] HorseAges = 
        {
            YOUNG, ADOLESCENT, MATURE, AGED, OLD, VENERABLE
        };

        public static string[] HorseAgesUpcase;

        /// <summary>
        /// check for these names is ToUpperInvariant
        /// </summary>
        static string[] WildCreatureNames = 
        {
            "Horse", "Hell horse", "deer", "cow", "bull", "bison", 
        };

        public static string[] OtherNamePrefixes =
        {
            FAT, STARVING, DISEASED
        };

        public static string[] AllNamePrefixes;

        static GrangerHelpers()
        {
            HorseAgesUpcase = HorseAges.Select(x => x.ToUpperInvariant()).ToArray();

            var allnameprefixesBuilder = new List<string>();
            allnameprefixesBuilder.AddRange(HorseAges);
            allnameprefixesBuilder.AddRange(OtherNamePrefixes);
            AllNamePrefixes = allnameprefixesBuilder.ToArray<string>();
        }

        public static string RemoveAllPrefixes(string horseName)
        {
            foreach (string prefix in AllNamePrefixes)
            {
                horseName = horseName.Replace(prefix, string.Empty);
            }
            horseName = horseName.Trim();
            return horseName;
        }

        /// <summary>
        /// capitalizes lower-case horse name
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

        public static HorseAge ExtractHorseAge(string prefixedObjectName)
        {
            return HorseAge.CreateAgeFromRawHorseNameStartsWith(prefixedObjectName);
        }

        /// <summary>
        /// Checks if provided string contains blacklisted (wild) creature name
        /// </summary>
        /// <param name="prefixedObjectName"></param>
        /// <returns></returns>
        public static bool IsBlacklistedCreatureName(string prefixedObjectName)
        {
            // allow adding any creature
            return false;

            //prefixedObjectName = prefixedObjectName.ToUpperInvariant();
            //foreach (string name in WildCreatureNames)
            //{
            //    if (prefixedObjectName.Contains(name.ToUpperInvariant())) return true;
            //}
            //return false;
        }

        /// <summary>
        /// Checks if provided string is EQUAL to blacklisted (wild) creature name.
        /// </summary>
        /// <param name="fixedHorseName"></param>
        /// <returns></returns>
        public static bool IsBlacklistedCreatureName_EqualCheck(string fixedHorseName)
        {
            fixedHorseName = fixedHorseName.ToUpperInvariant();
            foreach (string name in WildCreatureNames)
            {
                if (fixedHorseName == name.ToUpperInvariant()) return true;
            }
            return false;
        }

        public static bool HasAgeInName(string prefixedObjectName, bool ignoreCase = false)
        {
            foreach (string age in HorseAges)
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

        public static string ExtractHorseName(string prefixedObjectName)
        {
            return RemoveAllPrefixes(prefixedObjectName);
        }

        /// <summary>
        /// Returns possible horse name if line contains DISEASED. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsDiseased(string inputLine)
        {
            // try to match for [age] diseased [horsename]
            return LineContains(inputLine, DISEASED);
        }

        /// <summary>
        /// Returns possible horse name if line contains FAT. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsFat(string inputLine)
        {
            // try to match for [age] diseased [horsename]
            return LineContains(inputLine, FAT);
        }

        /// <summary>
        /// Returns possible horse name if line contains STARVING. Null if no matches.
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        public static string LineContainsStarving(string inputLine)
        {
            // try to match for [age] diseased [horsename]
            return LineContains(inputLine, STARVING);
        }

        static string LineContains(string input, string value)
        {
            Match match = Regex.Match(input, value + @" (\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success)
            {
                string possibleHorseName = match.Groups[1].Value;
                return FixCase(possibleHorseName);
            }
            else return null;
        }

        public static HorseTrait[] GetTraitsFromLine(string line)
        {
            List<HorseTrait> result = new List<HorseTrait>();
            foreach (var trait in HorseTrait.GetAllPossibleTraits())
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
