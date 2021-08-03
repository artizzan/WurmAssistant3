using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class CreatureTrait
    {
        static class Helper
        {
            const int TraitLineRecognitionCharCount = 3;

            static readonly Dictionary<string, CreatureTraitId> NameToEnumMap = new Dictionary<string, CreatureTraitId>();
            static readonly Dictionary<CreatureTraitId, string> EnumToNameMap = new Dictionary<CreatureTraitId, string>();
            static readonly Dictionary<CreatureTraitId, string> EnumToCompactNameMap = new Dictionary<CreatureTraitId, string>();
            static readonly Dictionary<CreatureTraitId, string> EnumToShortcutMap = new Dictionary<CreatureTraitId, string>();

            static readonly HashSet<string> TraitLineRecognitionSet = new HashSet<string>();

            static readonly Dictionary<CreatureTraitId, int> DefaultTraitValues = new Dictionary<CreatureTraitId, int>();

            static Helper()
            {
                MapTrait(
                    "Unknown", 
                    CreatureTraitId.Unknown, 
                    "Unk", 
                    "?", 
                    0, 
                    TraitType.Unknown, 
                    false, 
                    false, 
                    "Unknown trait");

                MapTrait(
                    "It will fight fiercely", 
                    CreatureTraitId.FightFiercely, 
                    "Fierce fight", 
                    "FF", 
                    10, 
                    TraitType.Combat, 
                    false, 
                    false, 
                    "Higher fighting skill");

                MapTrait(
                    "It has fleeter movement than normal",
                    CreatureTraitId.FleeterMovement,
                    "Fleet move",
                    "FM",
                    10,
                    TraitType.Speed,
                    false,
                    false,
                    "Minor speed boost");

                MapTrait(
                    "It is a tough bugger",
                    CreatureTraitId.ToughBugger,
                    "Tough bugger",
                    "TB",
                    15,
                    TraitType.Combat,
                    false,
                    false,
                    "Withstands more damage");

                MapTrait(
                    "It has a strong body",
                    CreatureTraitId.StrongBody,
                    "Strong body",
                    "SB",
                    15,
                    TraitType.Draft,
                    false,
                    false,
                    "Bonus to mounted weight limit");

                MapTrait(
                    "It has lightning movement",
                    CreatureTraitId.LightningMovement,
                    "Light move",
                    "LM",
                    20,
                    TraitType.Speed,
                    false,
                    false,
                    "Major speed boost");

                MapTrait(
                    "It can carry more than average",
                    CreatureTraitId.CarryMoreThanAverage,
                    "Carry more",
                    "CM",
                    20,
                    TraitType.Draft,
                    false,
                    false,
                    "Major bonus to mounted weight limit");

                MapTrait(
                    "It has very strong leg muscles",
                    CreatureTraitId.VeryStrongLegs,
                    "V. Strong legs",
                    "VSL",
                    15,
                    TraitType.Speed,
                    false,
                    false,
                    "Movement speed bonus");

                MapTrait(
                    "It has keen senses",
                    CreatureTraitId.KeenSenses,
                    "Keen senses",
                    "KS",
                    0,
                    TraitType.Unknown,
                    false,
                    false,
                    "Unknown effect");

                MapTrait(
                    "It has malformed hindlegs",
                    CreatureTraitId.MalformedHindlegs,
                    "Malf. hindlegs",
                    "MH",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Minor speed penalty");

                MapTrait(
                    "The legs are of different length",
                    CreatureTraitId.LegsOfDifferentLength,
                    "Diff. legs",
                    "DL",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Major speed penalty");

                MapTrait(
                    "It seems overly aggressive",
                    CreatureTraitId.OverlyAggressive,
                    "Agressive",
                    "OA",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Random chance to bite");

                MapTrait(
                    "It looks very unmotivated",
                    CreatureTraitId.VeryUnmotivated,
                    "Unmotivated",
                    "UM",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Major penalty to mounted weight limit");

                MapTrait(
                    "It is unusually strong willed",
                    CreatureTraitId.UnusuallyStrongWilled,
                    "Strong will",
                    "SW",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Will stop being led at random");

                MapTrait(
                    "It has some illness",
                    CreatureTraitId.HasSomeIllness,
                    "Ilness",
                    "SI",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Body strength will slowly reduce over time eventually making the animal unrideable");

                MapTrait(
                    "It looks constantly hungry",
                    CreatureTraitId.ConstantlyHungry,
                    "Hungry",
                    "CH",
                    5,
                    TraitType.Combat,
                    false,
                    true,
                    "Becomes hungry twice as fast as normal");

                MapTrait(
                    "It looks feeble and unhealthy",
                    CreatureTraitId.FeebleAndUnhealthy,
                    "Feeble",
                    "FU",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Prone to catching a disease");

                MapTrait(
                    "It looks unusually strong and healthy",
                    CreatureTraitId.StrongAndHealthy,
                    "Healthy",
                    "SH",
                    10,
                    TraitType.Misc,
                    false,
                    false,
                    "Has a higher resistance to disease");

                MapTrait(
                    "It has a certain spark in its eyes",
                    CreatureTraitId.CertainSpark,
                    "Spark",
                    "CS",
                    10,
                    TraitType.Misc,
                    false,
                    false,
                    "Lives 50% longer than normal");

                MapTrait(
                    "It seems extremely tame",
                    CreatureTraitId.SeemsExtremelyTame,
                    "Extremely tame",
                    "SET",
                    0,
                    TraitType.Combat,
                    true,
                    false,
                    "Aggressive animals become passive");

                MapTrait(
                    "It seems more friendly",
                    CreatureTraitId.SeemsMoreFriendly,
                    "More friendly",
                    "SMF",
                    5,
                    TraitType.Combat,
                    false,
                    false,
                    "Easier to tame");

                MapTrait(
                    "It looks more friendly than normal",
                    CreatureTraitId.LooksMoreFriendlyThanNormal,
                    "More friendly",
                    "MFTN",
                    15,
                    TraitType.Combat,
                    false,
                    false,
                    "Less likely to be attacked by aggressive creatures when tame");

                MapTrait(
                    "It seems especially loyal",
                    CreatureTraitId.SeemsEspeciallyLoyal,
                    "Especially loyal",
                    "SEL",
                    20,
                    TraitType.Combat,
                    false,
                    false,
                    "keeps loyalty to its tamer longer, loses less when taking damage");

                MapTrait(
                    "It seems stronger than normal",
                    CreatureTraitId.SeemsStrongerThanNormal,
                    "Stronger",
                    "SSTN",
                    0,
                    TraitType.Draft,
                    true,
                    false,
                    "Carry weight bonus");

                MapTrait(
                    "It seems more nimble than normal",
                    CreatureTraitId.SeemsMoreNimbleThanNormal,
                    "More nimble",
                    "MNTN",
                    0,
                    TraitType.Draft,
                    true,
                    false,
                    "Increased maximum rideable slope");

                MapTrait(
                    "It is easy on its gear",
                    CreatureTraitId.IsEasyOnItsGear,
                    "Easy on gear",
                    "EOIG",
                    10,
                    TraitType.Draft,
                    false,
                    false,
                    "Equipped gear takes less damage");

                MapTrait(
                    "It has strong legs",
                    CreatureTraitId.HasStrongLegs,
                    "Strong legs",
                    "HSL",
                    20,
                    TraitType.Draft,
                    false,
                    false,
                    "Carry weight bonus");

                MapTrait(
                    "Bred in captivity",
                    CreatureTraitId.BredInCaptivity,
                    "Bred captive",
                    "BC",
                    0,
                    TraitType.Misc,
                    false,
                    false,
                    "Informational, will not count toward the max number of traits");

                MapTrait(
                    "It has a chance to produce twins",
                    CreatureTraitId.HasChanceToProduceTwins,
                    "Produce twins",
                    "CPT",
                    0,
                    TraitType.Misc,
                    true,
                    false,
                    "Chance to birth twins");

                MapTrait(
                    "It seems immortal",
                    CreatureTraitId.SeemsImmortal,
                    "Immortal",
                    "IM",
                    0,
                    TraitType.Misc,
                    true,
                    true,
                    "Will never die as if cared for");

                MapTrait(
                    "Horse's color is considered a trait",
                    CreatureTraitId.ColorIsConsideredTrait,
                    "Color is trait",
                    "CCT",
                    0,
                    TraitType.Misc,
                    false,
                    false,
                    "Does not count against the trait limit");

                MapTrait(
                    "It has been corrupted",
                    CreatureTraitId.HasBeenCorrupted,
                    "Corrupted",
                    "HBC",
                    0,
                    TraitType.Misc,
                    false,
                    false,
                    "Grazes on mycelium instead of grass");

                MapTrait(
                    "It has a slow metabolism",
                    CreatureTraitId.HasSlowMetabolism,
                    "Slow metabolism",
                    "HSM",
                    10,
                    TraitType.Misc,
                    false,
                    false,
                    "Eats half as much as a normal animal");

                MapTrait(
                    "It looks stationary",
                    CreatureTraitId.SeemsStationary,
                    "Stationary",
                    "SS",
                    10,
                    TraitType.Misc,
                    false,
                    false,
                    "It will stay put as if saddled");

                MapTrait(
                    "It seems to be a graceful eater",
                    CreatureTraitId.SeemsToBeGracefulEater,
                    "Graceful eater",
                    "GE",
                    10,
                    TraitType.Misc,
                    false,
                    false,
                    "Less chance to reduce the growth stage of a tile when eating");

                MapTrait(
                    "It looks extremely sick",
                    CreatureTraitId.LooksExtremelySick,
                    "Extremely sick",
                    "ES",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Has a very slim chance to pass away when it receives a hunger tick");

                MapTrait(
                    "It seems shabby and frail",
                    CreatureTraitId.SeemsShabbyAndFrail,
                    "Shabby n frail",
                    "SF",
                    5,
                    TraitType.Negative,
                    false,
                    false,
                    "Reduces output of resources such as milk and wool");

                MapTrait(
                    "It seems to dislike steep terrain",
                    CreatureTraitId.SeemsToDislikeSteepTerrain,
                    "Dislike slopes",
                    "DST",
                    5,
                    TraitType.Negative,
                    false,
                    true,
                    "Decreases ridable slope");

                MapTrait(
                    "It has very good genes",
                    CreatureTraitId.HasVeryGoodGenes,
                    "Good genes",
                    "VGG",
                    0,
                    TraitType.Output,
                    true,
                    false,
                    "Increased amount and quality of resources like milk and wool");

                MapTrait(
                    "It seems to pick stuff up",
                    CreatureTraitId.SeemsToPickStuffUp,
                    "Picks stuff",
                    "PSU",
                    5,
                    TraitType.Output,
                    false,
                    false,
                    "Chance to dig something up when eating, drops items on the ground");

                MapTrait(
                    "It seems vibrant",
                    CreatureTraitId.SeemsVibrant,
                    "Vibrant",
                    "SV",
                    5,
                    TraitType.Output,
                    false,
                    false,
                    "Increases the output of resources");

                MapTrait(
                    "It seems prize winning",
                    CreatureTraitId.SeemsPrizeWinning,
                    "Prize winning",
                    "SPW",
                    10,
                    TraitType.Output,
                    false,
                    false,
                    "Gives better products when butchered");

                MapTrait(
                    "It gives more resources",
                    CreatureTraitId.GivesMoreResources,
                    "More resources",
                    "GMR",
                    10,
                    TraitType.Output,
                    false,
                    false,
                    "Increases output of resources such as wool and milk");

                MapTrait(
                    "It looks plump and ready to butcher",
                    CreatureTraitId.LooksPlumpAndReadyToButcher,
                    "Rdy to butcher",
                    "PRB",
                    20,
                    TraitType.Output,
                    false,
                    false,
                    "Gives more products when butchered");

                MapTrait(
                    "It seems accustomed to water",
                    CreatureTraitId.SeemsAccustomedToWater,
                    "Loves water",
                    "AW",
                    10,
                    TraitType.Speed,
                    false,
                    false,
                    "Moves faster in shallow waters");

                MapTrait(
                    "It is unbelievably fast",
                    CreatureTraitId.IsUnbelievablyFast,
                    "Unbieliev fast",
                    "UF",
                    0,
                    TraitType.Speed,
                    true,
                    false,
                    "Always on speed bonus similar to hell horses");
            }

            static void MapTrait(string wurmText, CreatureTraitId creatureTraitId, string compactText, string shortcutText, 
                int traitAhCost, TraitType traitType, bool isRare, bool removedByGenesis, string effectDescription)
            {
                NameToEnumMap.Add(wurmText, creatureTraitId);
                EnumToNameMap.Add(creatureTraitId, wurmText);
                TraitLineRecognitionSet.Add(wurmText.Substring(0, TraitLineRecognitionCharCount));
                EnumToCompactNameMap.Add(creatureTraitId, compactText);
                EnumToShortcutMap.Add(creatureTraitId, shortcutText);
                DefaultTraitValues.Add(creatureTraitId, 0);
            }

            internal static CreatureTrait[] ExtractTraitsFromLine(string line)
            {
                List<CreatureTrait> traits = new List<CreatureTrait>();
                foreach (var keyval in NameToEnumMap)
                {
                    if (line.Contains(keyval.Key))
                    {
                        traits.Add(new CreatureTrait(keyval.Value));
                    }
                }
                return traits.ToArray<CreatureTrait>();
            }

            internal static bool CanThisBeTraitLine(string line)
            {
                return TraitLineRecognitionSet.Contains(line.Substring(0, TraitLineRecognitionCharCount));
            }

            internal static string GetTextForTrait(CreatureTraitId creatureTrait)
            {
                try
                {
                    return Granger.CreatureTrait.Helper.EnumToNameMap[creatureTrait];
                }
                catch (KeyNotFoundException)
                {
                    return "error";
                };
            }

            internal static CreatureTraitId GetEnumFromEnumStr(string enumStr)
            {
                try
                {
                    return (CreatureTraitId)Enum.Parse(typeof(CreatureTraitId), enumStr);
                }
                catch (ArgumentException)
                {
                    return CreatureTraitId.Unknown;
                }
            }

            internal static CreatureTraitId GetEnumFromEnumInt(string enumIntStr)
            {
                try
                {
                    return (CreatureTraitId)(int.Parse(enumIntStr == string.Empty ? "0" : enumIntStr));
                }
                catch (ArgumentException)
                {
                    return CreatureTraitId.Unknown;
                }
            }

            internal static int GetDefaultValue(CreatureTrait trait)
            {
                int result;
                if (!DefaultTraitValues.TryGetValue(trait.CreatureTraitId, out result))
                {
                    //todo
                    //Logger.LogError("no default value found for trait: " + trait.Trait.ToString());
                }
                return result;
            }

            internal static Dictionary<CreatureTraitId, int> GetAllDefaultValues()
            {
                return new Dictionary<CreatureTraitId, int>(DefaultTraitValues);
            }

            internal static string[] GetAllTraitWurmText()
            {
                return NameToEnumMap.Where(x => x.Value != CreatureTraitId.Unknown).Select(x => x.Key).ToArray();
            }

            internal static CreatureTraitId[] GetAllTraitEnums()
            {
                return NameToEnumMap.Where(x => x.Value != CreatureTraitId.Unknown).Select(x => x.Value).ToArray();
            }

            internal static CreatureTraitId GetEnumFromWurmTextRepr(string text)
            {
                return NameToEnumMap[text];
            }

            internal static string GetCompactNameForTrait(CreatureTrait creatureTrait)
            {
                return EnumToCompactNameMap[creatureTrait.CreatureTraitId];
            }

            internal static string GetShortcutForTrait(CreatureTrait trait, int value)
            {
                string prefix = string.Empty;
                if (value > 0) prefix = "+";
                if (value < 0) prefix = "-";
                return prefix + EnumToShortcutMap[trait.CreatureTraitId];
            }
        }

        public static class DbHelper
        {
            public static List<CreatureTrait> FromStrIntRepresentation(string strIntRepresentation)
            {
                if (strIntRepresentation == "TRAITLESS") return new List<CreatureTrait>();
                if (strIntRepresentation == null) strIntRepresentation = string.Empty;
                return new List<CreatureTrait>(strIntRepresentation.Split(',')
                    .Select(x => Granger.CreatureTrait.FromEnumIntStr(x)));
            }

            public static string ToIntStrRepresentation([NotNull] List<CreatureTrait> traits)
            {
                if (traits == null) throw new ArgumentNullException(nameof(traits));

                return traits.Count == 0
                    ? "TRAITLESS"
                    : string.Join(",", traits.Select(x => (int) x.CreatureTraitId));
            }
        }

        static CreatureTrait[] _cachedInbreedBadTraits;
        static CreatureTrait[] _cachedAllTraits;
        static CreatureTrait[] _cachedAllPossibleTraits;

        public CreatureTraitId CreatureTraitId { get; } = CreatureTraitId.Unknown;

        /// <summary>
        /// Create blank trait with Trait value of Unknown
        /// </summary>
        public CreatureTrait(CreatureTraitId enumval)
        {
            CreatureTraitId = enumval;
        }

        public static CreatureTrait FromEnumIntStr(string enumIntStr)
        {
            return new CreatureTrait(Helper.GetEnumFromEnumInt(enumIntStr));
        }

        internal static CreatureTrait FromWurmTextRepr(string text)
        {
            return new CreatureTrait(Helper.GetEnumFromWurmTextRepr(text));
        }

        public static int GetDefaultValue(CreatureTrait trait)
        {
            return Helper.GetDefaultValue(trait);
        }

        public static string[] GetAllTraitWurmText()
        {
            return Helper.GetAllTraitWurmText();
        }

        public static CreatureTraitId[] GetAllTraitEnums()
        {
            return Helper.GetAllTraitEnums();
        }

        public int ToInt32()
        {
            return (int)CreatureTraitId;
        }

        /// <summary>
        /// Get value of this trait within given context.
        /// </summary>
        /// <param name="traitvaluator"></param>
        /// <returns></returns>
        public int GetTraitValue(TraitValuator traitvaluator)
        {
            return traitvaluator.GetValueForTrait(this);
        }

        /// <summary>
        /// Returns dictionary of default trait values.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<CreatureTraitId, int> GetAllDefaultValues()
        {
            return Helper.GetAllDefaultValues();
        }

        internal static object GetWurmTextForTrait(CreatureTraitId creatureTrait)
        {
            return Helper.GetTextForTrait(creatureTrait);
        }

        internal static string GetShortString(CreatureTrait[] traits, TraitValuator valuator)
        {
            List<string> shorts = new List<string>();
            foreach (var trait in traits)
            {
                int value = 0;
                if (valuator != null) value = valuator.GetValueForTrait(trait);
                shorts.Add(Helper.GetShortcutForTrait(trait, value));
            }
            return string.Join(",", shorts.OrderBy(x => x));
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            CreatureTrait p = obj as CreatureTrait;
            if ((System.Object)p == null)
            {
                return false;
            }
            
            return this.CreatureTraitId == p.CreatureTraitId;
        }

        public bool Equals(CreatureTrait p)
        {
            if ((object)p == null)
            {
                return false;
            }
            
            return this.CreatureTraitId == p.CreatureTraitId;
        }

        public override int GetHashCode()
        {
            return (int)this.CreatureTraitId;
        }

        /// <summary>
        /// Wurm game client text description of this trait.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Helper.GetTextForTrait(CreatureTraitId);
        }

        internal string ToCompactString()
        {
            return Helper.GetCompactNameForTrait(this);
        }

        /// <summary>
        /// 2-letter shortcut for this trait
        /// </summary>
        /// <returns></returns>
        internal string ToShortcutString()
        {
            return Helper.GetShortcutForTrait(this, 0);
        }

        internal static CreatureTrait[] GetGoodTraits(CreatureTrait[] traits, TraitValuator traitValuator)
        {
            return traits.Where(x => x.GetTraitValue(traitValuator) > 0).ToArray();
        }

        internal static CreatureTrait[] GetBadTraits(CreatureTrait[] traits, TraitValuator traitValuator)
        {
            return traits.Where(x => x.GetTraitValue(traitValuator) < 0).ToArray();
        }

        internal static CreatureTrait[] GetInbreedBadTraits()
        {
            if (_cachedInbreedBadTraits == null)
                _cachedInbreedBadTraits = new CreatureTrait[]
                    {
                        new CreatureTrait(CreatureTraitId.ConstantlyHungry),
                        new CreatureTrait(CreatureTraitId.FeebleAndUnhealthy),
                        new CreatureTrait(CreatureTraitId.HasSomeIllness),
                        new CreatureTrait(CreatureTraitId.LegsOfDifferentLength),
                        new CreatureTrait(CreatureTraitId.VeryUnmotivated),
                    };
            return _cachedInbreedBadTraits;
        }

        /// <summary>
        /// includes the "unknown" trait
        /// </summary>
        /// <returns></returns>
        internal static CreatureTrait[] GetAllTraits()
        {
            if (_cachedAllTraits == null)
                _cachedAllTraits = ((CreatureTraitId[])Enum.GetValues(typeof(CreatureTraitId))).Select(x => new CreatureTrait(x)).ToArray();
            return _cachedAllTraits;
        }

        /// <summary>
        /// excludes the "unknown" trait
        /// </summary>
        /// <returns></returns>
        internal static CreatureTrait[] GetAllPossibleTraits()
        {
            if (_cachedAllPossibleTraits == null)
                _cachedAllPossibleTraits = GetAllTraits().Where(x => x.CreatureTraitId != CreatureTraitId.Unknown).ToArray();
            return _cachedAllPossibleTraits;
        }

        internal static bool CanThisBeTraitLogMessage(string message)
        {
            return Helper.CanThisBeTraitLine(message);
        }
    }
}
