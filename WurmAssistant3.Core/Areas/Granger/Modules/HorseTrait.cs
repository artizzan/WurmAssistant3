using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.ValuePreset;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    public class HorseTrait
    {
        //note: enum numbering is used for database persistence and must be maintained after publish!
        public enum TraitEnum
        {
            Unknown = 0,
            FightFiercely = 1,
            FleeterMovement = 2,
            ToughBugger = 3,
            StrongBody = 4,
            LightningMovement = 5,
            CarryMoreThanAverage = 6,
            VeryStrongLegs = 7,
            KeenSenses = 8,
            MalformedHindlegs = 9,
            LegsOfDifferentLength = 10,
            OverlyAggressive = 11,
            VeryUnmotivated = 12,
            UnusuallyStrongWilled = 13,
            HasSomeIllness = 14,
            ConstantlyHungry = 15,
            FeebleAndUnhealthy = 16,
            StrongAndHealthy = 17,
            CertainSpark = 18
        }

        static class Helper
        {
            const int TraitLineRecognitionCharCount = 3;

            static readonly Dictionary<string, TraitEnum> NameToEnumMap = new Dictionary<string, TraitEnum>();
            static readonly Dictionary<TraitEnum, string> EnumToNameMap = new Dictionary<TraitEnum, string>();
            static readonly Dictionary<TraitEnum, string> EnumToCompactNameMap = new Dictionary<TraitEnum, string>();
            static readonly Dictionary<TraitEnum, string> EnumToShortcutMap = new Dictionary<TraitEnum, string>();
            static readonly Dictionary<TraitEnum, float> EnumToAhSkillMapFreedom = new Dictionary<TraitEnum, float>();
            static readonly Dictionary<TraitEnum, float> EnumToAhSkillMapEpic = new Dictionary<TraitEnum, float>();

            static readonly HashSet<string> TraitLineRecognitionSet = new HashSet<string>();

            static readonly Dictionary<TraitEnum, int> DefaultTraitValues = new Dictionary<TraitEnum, int>();

            static Helper()
            {
                NameToEnumMap.Add("Unknown", TraitEnum.Unknown);
                EnumToCompactNameMap.Add(TraitEnum.Unknown, "Unk");
                EnumToShortcutMap.Add(TraitEnum.Unknown, "?");

                NameToEnumMap.Add("It will fight fiercely", TraitEnum.FightFiercely);
                EnumToCompactNameMap.Add(TraitEnum.FightFiercely, "Fierce fight");
                EnumToShortcutMap.Add(TraitEnum.FightFiercely, "FF");

                NameToEnumMap.Add("It has fleeter movement than normal", TraitEnum.FleeterMovement);
                EnumToCompactNameMap.Add(TraitEnum.FleeterMovement, "Fleet move");
                EnumToShortcutMap.Add(TraitEnum.FleeterMovement, "FM");

                NameToEnumMap.Add("It is a tough bugger", TraitEnum.ToughBugger);
                EnumToCompactNameMap.Add(TraitEnum.ToughBugger, "Tough bugger");
                EnumToShortcutMap.Add(TraitEnum.ToughBugger, "TB");

                NameToEnumMap.Add("It has a strong body", TraitEnum.StrongBody);
                EnumToCompactNameMap.Add(TraitEnum.StrongBody, "Strong body");
                EnumToShortcutMap.Add(TraitEnum.StrongBody, "SB");

                NameToEnumMap.Add("It has lightning movement", TraitEnum.LightningMovement);
                EnumToCompactNameMap.Add(TraitEnum.LightningMovement, "Light move");
                EnumToShortcutMap.Add(TraitEnum.LightningMovement, "LM");

                NameToEnumMap.Add("It can carry more than average", TraitEnum.CarryMoreThanAverage);
                EnumToCompactNameMap.Add(TraitEnum.CarryMoreThanAverage, "Carry more");
                EnumToShortcutMap.Add(TraitEnum.CarryMoreThanAverage, "CM");

                NameToEnumMap.Add("It has very strong leg muscles", TraitEnum.VeryStrongLegs);
                EnumToCompactNameMap.Add(TraitEnum.VeryStrongLegs, "Strong legs");
                EnumToShortcutMap.Add(TraitEnum.VeryStrongLegs, "SL");

                NameToEnumMap.Add("It has keen senses", TraitEnum.KeenSenses);
                EnumToCompactNameMap.Add(TraitEnum.KeenSenses, "Keen senses");
                EnumToShortcutMap.Add(TraitEnum.KeenSenses, "KS");

                NameToEnumMap.Add("It has malformed hindlegs", TraitEnum.MalformedHindlegs);
                EnumToCompactNameMap.Add(TraitEnum.MalformedHindlegs, "Malf. hindlegs");
                EnumToShortcutMap.Add(TraitEnum.MalformedHindlegs, "MH");

                NameToEnumMap.Add("The legs are of different length", TraitEnum.LegsOfDifferentLength);
                EnumToCompactNameMap.Add(TraitEnum.LegsOfDifferentLength, "Diff. legs");
                EnumToShortcutMap.Add(TraitEnum.LegsOfDifferentLength, "DL");

                NameToEnumMap.Add("It seems overly aggressive", TraitEnum.OverlyAggressive);
                EnumToCompactNameMap.Add(TraitEnum.OverlyAggressive, "Agressive");
                EnumToShortcutMap.Add(TraitEnum.OverlyAggressive, "OA");

                NameToEnumMap.Add("It looks very unmotivated", TraitEnum.VeryUnmotivated);
                EnumToCompactNameMap.Add(TraitEnum.VeryUnmotivated, "Unmotivated");
                EnumToShortcutMap.Add(TraitEnum.VeryUnmotivated, "UM");

                NameToEnumMap.Add("It is unusually strong willed", TraitEnum.UnusuallyStrongWilled);
                EnumToCompactNameMap.Add(TraitEnum.UnusuallyStrongWilled, "Strong will");
                EnumToShortcutMap.Add(TraitEnum.UnusuallyStrongWilled, "SW");

                NameToEnumMap.Add("It has some illness", TraitEnum.HasSomeIllness);
                EnumToCompactNameMap.Add(TraitEnum.HasSomeIllness, "Ilness");
                EnumToShortcutMap.Add(TraitEnum.HasSomeIllness, "SL");

                NameToEnumMap.Add("It looks constantly hungry", TraitEnum.ConstantlyHungry);
                EnumToCompactNameMap.Add(TraitEnum.ConstantlyHungry, "Hungry");
                EnumToShortcutMap.Add(TraitEnum.ConstantlyHungry, "CH");

                NameToEnumMap.Add("It looks feeble and unhealthy", TraitEnum.FeebleAndUnhealthy);
                EnumToCompactNameMap.Add(TraitEnum.FeebleAndUnhealthy, "Feeble");
                EnumToShortcutMap.Add(TraitEnum.FeebleAndUnhealthy, "FU");

                NameToEnumMap.Add("It looks unusually strong and healthy", TraitEnum.StrongAndHealthy);
                EnumToCompactNameMap.Add(TraitEnum.StrongAndHealthy, "Healthy");
                EnumToShortcutMap.Add(TraitEnum.StrongAndHealthy, "SH");

                NameToEnumMap.Add("It has a certain spark in its eyes", TraitEnum.CertainSpark);
                EnumToCompactNameMap.Add(TraitEnum.CertainSpark, "Spark");
                EnumToShortcutMap.Add(TraitEnum.CertainSpark, "CS");

                foreach (var keyval in NameToEnumMap)
                {
                    EnumToNameMap.Add(keyval.Value, keyval.Key);
                    TraitLineRecognitionSet.Add(keyval.Key.Substring(0, TraitLineRecognitionCharCount));
                }

                EnumToAhSkillMapFreedom.Add(TraitEnum.Unknown, 0);
                EnumToAhSkillMapEpic.Add(TraitEnum.Unknown, 0);
                DefaultTraitValues.Add(TraitEnum.Unknown, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.FightFiercely, 20);
                EnumToAhSkillMapEpic.Add(TraitEnum.FightFiercely, 10.56F);
                DefaultTraitValues.Add(TraitEnum.FightFiercely, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.FleeterMovement, 21);
                EnumToAhSkillMapEpic.Add(TraitEnum.FleeterMovement, 11.12F);
                DefaultTraitValues.Add(TraitEnum.FleeterMovement, 60);

                EnumToAhSkillMapFreedom.Add(TraitEnum.ToughBugger, 22);
                EnumToAhSkillMapEpic.Add(TraitEnum.ToughBugger, 11.68F);
                DefaultTraitValues.Add(TraitEnum.ToughBugger, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.StrongBody, 23);
                EnumToAhSkillMapEpic.Add(TraitEnum.StrongBody, 12.25F);
                DefaultTraitValues.Add(TraitEnum.StrongBody, 40);

                EnumToAhSkillMapFreedom.Add(TraitEnum.LightningMovement, 24);
                EnumToAhSkillMapEpic.Add(TraitEnum.LightningMovement, 12.82F);
                DefaultTraitValues.Add(TraitEnum.LightningMovement, 80);

                EnumToAhSkillMapFreedom.Add(TraitEnum.CarryMoreThanAverage, 25);
                EnumToAhSkillMapEpic.Add(TraitEnum.CarryMoreThanAverage, 13.40F);
                DefaultTraitValues.Add(TraitEnum.CarryMoreThanAverage, 50);

                EnumToAhSkillMapFreedom.Add(TraitEnum.VeryStrongLegs, 26);
                EnumToAhSkillMapEpic.Add(TraitEnum.VeryStrongLegs, 13.98F);
                DefaultTraitValues.Add(TraitEnum.VeryStrongLegs, 60);

                EnumToAhSkillMapFreedom.Add(TraitEnum.KeenSenses, 27);
                EnumToAhSkillMapEpic.Add(TraitEnum.KeenSenses, 14.56F);
                DefaultTraitValues.Add(TraitEnum.KeenSenses, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.MalformedHindlegs, 28);
                EnumToAhSkillMapEpic.Add(TraitEnum.MalformedHindlegs, 15.15F);
                DefaultTraitValues.Add(TraitEnum.MalformedHindlegs, -80);

                EnumToAhSkillMapFreedom.Add(TraitEnum.LegsOfDifferentLength, 29);
                EnumToAhSkillMapEpic.Add(TraitEnum.LegsOfDifferentLength, 15.74F);
                DefaultTraitValues.Add(TraitEnum.LegsOfDifferentLength, -100);

                EnumToAhSkillMapFreedom.Add(TraitEnum.OverlyAggressive, 30);
                EnumToAhSkillMapEpic.Add(TraitEnum.OverlyAggressive, 16.33F);
                DefaultTraitValues.Add(TraitEnum.OverlyAggressive, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.VeryUnmotivated, 31);
                EnumToAhSkillMapEpic.Add(TraitEnum.VeryUnmotivated, 16.93F);
                DefaultTraitValues.Add(TraitEnum.VeryUnmotivated, -80);

                EnumToAhSkillMapFreedom.Add(TraitEnum.UnusuallyStrongWilled, 32);
                EnumToAhSkillMapEpic.Add(TraitEnum.UnusuallyStrongWilled, 17.54F);
                DefaultTraitValues.Add(TraitEnum.UnusuallyStrongWilled, 0);

                EnumToAhSkillMapFreedom.Add(TraitEnum.HasSomeIllness, 33);
                EnumToAhSkillMapEpic.Add(TraitEnum.HasSomeIllness, 18.15F);
                DefaultTraitValues.Add(TraitEnum.HasSomeIllness, -150);

                EnumToAhSkillMapFreedom.Add(TraitEnum.ConstantlyHungry, 34);
                EnumToAhSkillMapEpic.Add(TraitEnum.ConstantlyHungry, 18.76F);
                DefaultTraitValues.Add(TraitEnum.ConstantlyHungry, -20);

                EnumToAhSkillMapFreedom.Add(TraitEnum.FeebleAndUnhealthy, 39);
                EnumToAhSkillMapEpic.Add(TraitEnum.FeebleAndUnhealthy, 21.90F);
                DefaultTraitValues.Add(TraitEnum.FeebleAndUnhealthy, -10);

                EnumToAhSkillMapFreedom.Add(TraitEnum.StrongAndHealthy, 40);
                EnumToAhSkillMapEpic.Add(TraitEnum.StrongAndHealthy, 22.54F);
                DefaultTraitValues.Add(TraitEnum.StrongAndHealthy, 10);

                EnumToAhSkillMapFreedom.Add(TraitEnum.CertainSpark, 41);
                EnumToAhSkillMapEpic.Add(TraitEnum.CertainSpark, 23.19F);
                DefaultTraitValues.Add(TraitEnum.CertainSpark, 0);
            }

            internal static float MinSkillForTheseTraits(HorseTrait[] traits, bool epicCurve)
            {
                float result = 0;
                foreach (var trait in traits)
                {
                    float skillForThisTrait = GetAhSkillForThisTrait(trait, epicCurve);

                    if (skillForThisTrait > result) result = skillForThisTrait;
                }
                return result;
            }

            internal static float GetAhSkillForThisTrait(HorseTrait horseTrait, bool epicCurve)
            {
                if (epicCurve) return EnumToAhSkillMapEpic[horseTrait.Trait];
                else return EnumToAhSkillMapFreedom[horseTrait.Trait];
            }

            internal static HorseTrait[] ExtractTraitsFromLine(string line)
            {
                List<HorseTrait> traits = new List<HorseTrait>();
                foreach (var keyval in NameToEnumMap)
                {
                    if (line.Contains(keyval.Key))
                    {
                        traits.Add(new HorseTrait(keyval.Value));
                    }
                }
                return traits.ToArray<HorseTrait>();
            }

            internal static bool CanThisBeTraitLine(string line)
            {
                return TraitLineRecognitionSet.Contains(line.Substring(0, TraitLineRecognitionCharCount));
            }

            internal static string GetTextForTrait(TraitEnum trait)
            {
                try
                {
                    return HorseTrait.Helper.EnumToNameMap[trait];
                }
                catch (KeyNotFoundException)
                {
                    return "error";
                };
            }

            internal static TraitEnum GetEnumFromEnumStr(string enumStr)
            {
                try
                {
                    return (TraitEnum)Enum.Parse(typeof(TraitEnum), enumStr);
                }
                catch (ArgumentException)
                {
                    return TraitEnum.Unknown;
                }
            }

            internal static TraitEnum GetEnumFromEnumInt(string enumIntStr)
            {
                try
                {
                    return (TraitEnum)(int.Parse(enumIntStr == string.Empty ? "0" : enumIntStr));
                }
                catch (ArgumentException)
                {
                    return TraitEnum.Unknown;
                }
            }

            internal static int GetDefaultValue(HorseTrait trait)
            {
                int result;
                if (!DefaultTraitValues.TryGetValue(trait.Trait, out result))
                {
                    //todo
                    //Logger.LogError("no default value found for trait: " + trait.Trait.ToString());
                }
                return result;
            }

            internal static Dictionary<TraitEnum, int> GetAllDefaultValues()
            {
                return new Dictionary<TraitEnum, int>(DefaultTraitValues);
            }

            internal static string[] GetAllTraitWurmText()
            {
                return NameToEnumMap.Where(x => x.Value != TraitEnum.Unknown).Select(x => x.Key).ToArray();
            }

            internal static TraitEnum[] GetAllTraitEnums()
            {
                return NameToEnumMap.Where(x => x.Value != TraitEnum.Unknown).Select(x => x.Value).ToArray();
            }

            internal static TraitEnum GetEnumFromWurmTextRepr(string text)
            {
                return NameToEnumMap[text];
            }

            internal static string GetCompactNameForTrait(HorseTrait horseTrait)
            {
                return EnumToCompactNameMap[horseTrait.Trait];
            }

            internal static string GetShortcutForTrait(HorseTrait trait, int value)
            {
                string prefix = string.Empty;
                if (value > 0) prefix = "+";
                if (value < 0) prefix = "-";
                return prefix + EnumToShortcutMap[trait.Trait];
            }

            internal static HorseTrait[] GetMissingTraits(HorseTrait[] horseTraits, float inspectSkill, bool epicCurve)
            {
                var correctDict = (epicCurve ? EnumToAhSkillMapEpic : EnumToAhSkillMapFreedom);
                List<HorseTrait> missingTraits = new List<HorseTrait>();

                foreach (var keyval in correctDict)
                {
                    if (inspectSkill < keyval.Value && horseTraits.All(x => x.Trait != keyval.Key))
                        missingTraits.Add(new HorseTrait(keyval.Key));
                }

                return missingTraits.ToArray();
            }

            static float? _cachedMaxVisiblityCapEpic = null;
            static float? _cachedMaxVisiblityCapFreedom = null;

            internal static float GetFullTraitVisibilityCap(bool epicCurve)
            {
                if (epicCurve)
                {
                    if (_cachedMaxVisiblityCapEpic != null) return _cachedMaxVisiblityCapEpic.Value;
                    else
                    {
                        _cachedMaxVisiblityCapEpic = EnumToAhSkillMapEpic.Max(x => x.Value);
                        return _cachedMaxVisiblityCapEpic.Value;
                    }
                }
                else
                {
                    if (_cachedMaxVisiblityCapFreedom != null) return _cachedMaxVisiblityCapFreedom.Value;
                    else
                    {
                        _cachedMaxVisiblityCapFreedom = EnumToAhSkillMapFreedom.Max(x => x.Value);
                        return _cachedMaxVisiblityCapFreedom.Value;
                    }
                }
            }

            internal static HorseTrait[] GetTraitsUpToSkillLevel(float skillLevel, bool isEpic)
            {
                Dictionary<TraitEnum, float> correctDict;
                if (isEpic) correctDict = EnumToAhSkillMapEpic;
                else correctDict = EnumToAhSkillMapFreedom;
                List<HorseTrait> result = new List<HorseTrait>();
                foreach (var keyval in correctDict)
                {
                    if (keyval.Value < skillLevel) result.Add(new HorseTrait(keyval.Key));
                }
                return result.ToArray();
            }
        }

        public static class DbHelper
        {
            public static List<HorseTrait> FromStrIntRepresentation(string strINTRepresentation)
            {
                if (strINTRepresentation == "TRAITLESS") return new List<HorseTrait>();
                if (strINTRepresentation == null) strINTRepresentation = string.Empty;
                return new List<HorseTrait>(strINTRepresentation.Split(',')
                    .Select(x => HorseTrait.FromEnumIntStr(x)));
            }

            public static string ToIntStrRepresentation(List<HorseTrait> traits)
            {
                if (!(traits is List<HorseTrait>)) throw new ArgumentException("this is not HorseTrait list!");
                if (traits.Count == 0) return "TRAITLESS";
                return string.Join(",", traits.Select(x => (int)x.Trait));
            }
        }

        public readonly TraitEnum Trait = TraitEnum.Unknown;

        /// <summary>
        /// Create blank trait with Trait value of Unknown
        /// </summary>
        public HorseTrait(TraitEnum enumval)
        {
            Trait = enumval;
        }

        public static HorseTrait FromEnumIntStr(string enumIntStr)
        {
            return new HorseTrait(Helper.GetEnumFromEnumInt(enumIntStr));
        }

        internal static HorseTrait FromWurmTextRepr(string text)
        {
            return new HorseTrait(Helper.GetEnumFromWurmTextRepr(text));
        }

        public static int GetDefaultValue(HorseTrait trait)
        {
            return Helper.GetDefaultValue(trait);
        }

        public static string[] GetAllTraitWurmText()
        {
            return Helper.GetAllTraitWurmText();
        }

        public static TraitEnum[] GetAllTraitEnums()
        {
            return Helper.GetAllTraitEnums();
        }

        /// <summary>
        /// int32 id of this trait
        /// </summary>
        /// <returns></returns>
        public int ToInt32()
        {
            return (int)Trait;
        }

        /// <summary>
        /// Get value of this trait in given value context
        /// </summary>
        /// <param name="traitvaluator"></param>
        /// <returns></returns>
        public int GetTraitValue(TraitValuator traitvaluator)
        {
            return traitvaluator.GetValueForTrait(this);
        }

        /// <summary>
        /// Returns dictionary of default trait values
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TraitEnum, int> GetAllDefaultValues()
        {
            return Helper.GetAllDefaultValues();
        }

        internal static object GetWurmTextForTrait(TraitEnum trait)
        {
            return Helper.GetTextForTrait(trait);
        }

        internal static float GetMinSkillForTraits(HorseTrait[] traitlist, bool epicCurve)
        {
            return Helper.MinSkillForTheseTraits(traitlist, epicCurve);
        }

        internal static string GetShortString(HorseTrait[] traits, TraitValuator valuator)
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

        internal static HorseTrait[] GetMissingTraits(HorseTrait[] horseTraits, float inspectSkill, bool epicCurve)
        {
            return Helper.GetMissingTraits(horseTraits, inspectSkill, epicCurve);
        }

        internal bool IsUnknownForThisHorse(Horse horse)
        {
            if (HorseTrait.GetSkillForTrait(this, horse.EpicCurve) > horse.TraitsInspectSkill)
                return true;
            else return false;
        }

        private static float GetSkillForTrait(HorseTrait horseTrait, bool epicCurve)
        {
            return Helper.GetAhSkillForThisTrait(horseTrait, epicCurve);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            HorseTrait p = obj as HorseTrait;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Trait == p.Trait;
        }

        public bool Equals(HorseTrait p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Trait == p.Trait;
        }

        public override int GetHashCode()
        {
            return (int)this.Trait;
        }

        /// <summary>
        /// Text description of this trait, as visible in wurm event log
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Helper.GetTextForTrait(Trait);
        }

        /// <summary>
        /// Enum.ToString()
        /// </summary>
        /// <returns></returns>
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

        internal static HorseTrait[] GetGoodTraits(HorseTrait[] traits, TraitValuator traitValuator)
        {
            return traits.Where(x => x.GetTraitValue(traitValuator) > 0).ToArray();
        }

        internal static HorseTrait[] GetBadTraits(HorseTrait[] traits, TraitValuator traitValuator)
        {
            return traits.Where(x => x.GetTraitValue(traitValuator) < 0).ToArray();
        }

        static HorseTrait[] CachedInbreedBadTraits;

        internal static HorseTrait[] GetInbreedBadTraits()
        {
            if (CachedInbreedBadTraits == null)
                CachedInbreedBadTraits = new HorseTrait[]
                    {
                        new HorseTrait(TraitEnum.ConstantlyHungry),
                        new HorseTrait(TraitEnum.FeebleAndUnhealthy),
                        new HorseTrait(TraitEnum.HasSomeIllness),
                        new HorseTrait(TraitEnum.LegsOfDifferentLength),
                        new HorseTrait(TraitEnum.VeryUnmotivated),
                    };
            return CachedInbreedBadTraits;
        }

        internal static float GetFullTraitVisibilityCap(bool epicCurve)
        {
            return Helper.GetFullTraitVisibilityCap(epicCurve);
        }

        static HorseTrait[] CachedAllTraits;

        /// <summary>
        /// includes the "unknown" trait
        /// </summary>
        /// <returns></returns>
        internal static HorseTrait[] GetAllTraits()
        {
            if (CachedAllTraits == null)
                CachedAllTraits = ((TraitEnum[])Enum.GetValues(typeof(TraitEnum))).Select(x => new HorseTrait(x)).ToArray();
            return CachedAllTraits;
        }

        static HorseTrait[] CachedAllPossibleTraits;

        /// <summary>
        /// excludes the "unknown" trait
        /// </summary>
        /// <returns></returns>
        internal static HorseTrait[] GetAllPossibleTraits()
        {
            if (CachedAllPossibleTraits == null)
                CachedAllPossibleTraits = GetAllTraits().Where(x => x.Trait != TraitEnum.Unknown).ToArray();
            return CachedAllPossibleTraits;
        }

        internal static bool CanThisBeTraitLogMessage(string message)
        {
            return Helper.CanThisBeTraitLine(message);
        }

        internal static HorseTrait[] GetTraitsUpToSkillLevel(float skillLevel, bool isEpic)
        {
            return Helper.GetTraitsUpToSkillLevel(skillLevel, isEpic);
        }
    }
}
