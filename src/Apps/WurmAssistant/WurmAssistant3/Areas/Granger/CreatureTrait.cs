using System;
using System.Collections.Generic;
using System.Linq;
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
            static readonly Dictionary<CreatureTraitId, float> EnumToAhSkillMapFreedom = new Dictionary<CreatureTraitId, float>();
            static readonly Dictionary<CreatureTraitId, float> EnumToAhSkillMapEpic = new Dictionary<CreatureTraitId, float>();

            static readonly HashSet<string> TraitLineRecognitionSet = new HashSet<string>();

            static readonly Dictionary<CreatureTraitId, int> DefaultTraitValues = new Dictionary<CreatureTraitId, int>();

            static Helper()
            {
                NameToEnumMap.Add("Unknown", CreatureTraitId.Unknown);
                EnumToCompactNameMap.Add(CreatureTraitId.Unknown, "Unk");
                EnumToShortcutMap.Add(CreatureTraitId.Unknown, "?");

                NameToEnumMap.Add("It will fight fiercely", CreatureTraitId.FightFiercely);
                EnumToCompactNameMap.Add(CreatureTraitId.FightFiercely, "Fierce fight");
                EnumToShortcutMap.Add(CreatureTraitId.FightFiercely, "FF");

                NameToEnumMap.Add("It has fleeter movement than normal", CreatureTraitId.FleeterMovement);
                EnumToCompactNameMap.Add(CreatureTraitId.FleeterMovement, "Fleet move");
                EnumToShortcutMap.Add(CreatureTraitId.FleeterMovement, "FM");

                NameToEnumMap.Add("It is a tough bugger", CreatureTraitId.ToughBugger);
                EnumToCompactNameMap.Add(CreatureTraitId.ToughBugger, "Tough bugger");
                EnumToShortcutMap.Add(CreatureTraitId.ToughBugger, "TB");

                NameToEnumMap.Add("It has a strong body", CreatureTraitId.StrongBody);
                EnumToCompactNameMap.Add(CreatureTraitId.StrongBody, "Strong body");
                EnumToShortcutMap.Add(CreatureTraitId.StrongBody, "SB");

                NameToEnumMap.Add("It has lightning movement", CreatureTraitId.LightningMovement);
                EnumToCompactNameMap.Add(CreatureTraitId.LightningMovement, "Light move");
                EnumToShortcutMap.Add(CreatureTraitId.LightningMovement, "LM");

                NameToEnumMap.Add("It can carry more than average", CreatureTraitId.CarryMoreThanAverage);
                EnumToCompactNameMap.Add(CreatureTraitId.CarryMoreThanAverage, "Carry more");
                EnumToShortcutMap.Add(CreatureTraitId.CarryMoreThanAverage, "CM");

                NameToEnumMap.Add("It has very strong leg muscles", CreatureTraitId.VeryStrongLegs);
                EnumToCompactNameMap.Add(CreatureTraitId.VeryStrongLegs, "Strong legs");
                EnumToShortcutMap.Add(CreatureTraitId.VeryStrongLegs, "SL");

                NameToEnumMap.Add("It has keen senses", CreatureTraitId.KeenSenses);
                EnumToCompactNameMap.Add(CreatureTraitId.KeenSenses, "Keen senses");
                EnumToShortcutMap.Add(CreatureTraitId.KeenSenses, "KS");

                NameToEnumMap.Add("It has malformed hindlegs", CreatureTraitId.MalformedHindlegs);
                EnumToCompactNameMap.Add(CreatureTraitId.MalformedHindlegs, "Malf. hindlegs");
                EnumToShortcutMap.Add(CreatureTraitId.MalformedHindlegs, "MH");

                NameToEnumMap.Add("The legs are of different length", CreatureTraitId.LegsOfDifferentLength);
                EnumToCompactNameMap.Add(CreatureTraitId.LegsOfDifferentLength, "Diff. legs");
                EnumToShortcutMap.Add(CreatureTraitId.LegsOfDifferentLength, "DL");

                NameToEnumMap.Add("It seems overly aggressive", CreatureTraitId.OverlyAggressive);
                EnumToCompactNameMap.Add(CreatureTraitId.OverlyAggressive, "Agressive");
                EnumToShortcutMap.Add(CreatureTraitId.OverlyAggressive, "OA");

                NameToEnumMap.Add("It looks very unmotivated", CreatureTraitId.VeryUnmotivated);
                EnumToCompactNameMap.Add(CreatureTraitId.VeryUnmotivated, "Unmotivated");
                EnumToShortcutMap.Add(CreatureTraitId.VeryUnmotivated, "UM");

                NameToEnumMap.Add("It is unusually strong willed", CreatureTraitId.UnusuallyStrongWilled);
                EnumToCompactNameMap.Add(CreatureTraitId.UnusuallyStrongWilled, "Strong will");
                EnumToShortcutMap.Add(CreatureTraitId.UnusuallyStrongWilled, "SW");

                NameToEnumMap.Add("It has some illness", CreatureTraitId.HasSomeIllness);
                EnumToCompactNameMap.Add(CreatureTraitId.HasSomeIllness, "Ilness");
                EnumToShortcutMap.Add(CreatureTraitId.HasSomeIllness, "SL");

                NameToEnumMap.Add("It looks constantly hungry", CreatureTraitId.ConstantlyHungry);
                EnumToCompactNameMap.Add(CreatureTraitId.ConstantlyHungry, "Hungry");
                EnumToShortcutMap.Add(CreatureTraitId.ConstantlyHungry, "CH");

                NameToEnumMap.Add("It looks feeble and unhealthy", CreatureTraitId.FeebleAndUnhealthy);
                EnumToCompactNameMap.Add(CreatureTraitId.FeebleAndUnhealthy, "Feeble");
                EnumToShortcutMap.Add(CreatureTraitId.FeebleAndUnhealthy, "FU");

                NameToEnumMap.Add("It looks unusually strong and healthy", CreatureTraitId.StrongAndHealthy);
                EnumToCompactNameMap.Add(CreatureTraitId.StrongAndHealthy, "Healthy");
                EnumToShortcutMap.Add(CreatureTraitId.StrongAndHealthy, "SH");

                NameToEnumMap.Add("It has a certain spark in its eyes", CreatureTraitId.CertainSpark);
                EnumToCompactNameMap.Add(CreatureTraitId.CertainSpark, "Spark");
                EnumToShortcutMap.Add(CreatureTraitId.CertainSpark, "CS");

                foreach (var keyval in NameToEnumMap)
                {
                    EnumToNameMap.Add(keyval.Value, keyval.Key);
                    TraitLineRecognitionSet.Add(keyval.Key.Substring(0, TraitLineRecognitionCharCount));
                }

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.Unknown, 0);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.Unknown, 0);
                DefaultTraitValues.Add(CreatureTraitId.Unknown, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.FightFiercely, 20);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.FightFiercely, 10.56F);
                DefaultTraitValues.Add(CreatureTraitId.FightFiercely, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.FleeterMovement, 21);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.FleeterMovement, 11.12F);
                DefaultTraitValues.Add(CreatureTraitId.FleeterMovement, 60);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.ToughBugger, 22);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.ToughBugger, 11.68F);
                DefaultTraitValues.Add(CreatureTraitId.ToughBugger, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.StrongBody, 23);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.StrongBody, 12.25F);
                DefaultTraitValues.Add(CreatureTraitId.StrongBody, 40);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.LightningMovement, 24);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.LightningMovement, 12.82F);
                DefaultTraitValues.Add(CreatureTraitId.LightningMovement, 80);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.CarryMoreThanAverage, 25);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.CarryMoreThanAverage, 13.40F);
                DefaultTraitValues.Add(CreatureTraitId.CarryMoreThanAverage, 50);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.VeryStrongLegs, 26);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.VeryStrongLegs, 13.98F);
                DefaultTraitValues.Add(CreatureTraitId.VeryStrongLegs, 60);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.KeenSenses, 27);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.KeenSenses, 14.56F);
                DefaultTraitValues.Add(CreatureTraitId.KeenSenses, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.MalformedHindlegs, 28);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.MalformedHindlegs, 15.15F);
                DefaultTraitValues.Add(CreatureTraitId.MalformedHindlegs, -80);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.LegsOfDifferentLength, 29);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.LegsOfDifferentLength, 15.74F);
                DefaultTraitValues.Add(CreatureTraitId.LegsOfDifferentLength, -100);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.OverlyAggressive, 30);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.OverlyAggressive, 16.33F);
                DefaultTraitValues.Add(CreatureTraitId.OverlyAggressive, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.VeryUnmotivated, 31);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.VeryUnmotivated, 16.93F);
                DefaultTraitValues.Add(CreatureTraitId.VeryUnmotivated, -80);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.UnusuallyStrongWilled, 32);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.UnusuallyStrongWilled, 17.54F);
                DefaultTraitValues.Add(CreatureTraitId.UnusuallyStrongWilled, 0);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.HasSomeIllness, 33);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.HasSomeIllness, 18.15F);
                DefaultTraitValues.Add(CreatureTraitId.HasSomeIllness, -150);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.ConstantlyHungry, 34);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.ConstantlyHungry, 18.76F);
                DefaultTraitValues.Add(CreatureTraitId.ConstantlyHungry, -20);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.FeebleAndUnhealthy, 39);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.FeebleAndUnhealthy, 21.90F);
                DefaultTraitValues.Add(CreatureTraitId.FeebleAndUnhealthy, -10);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.StrongAndHealthy, 40);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.StrongAndHealthy, 22.54F);
                DefaultTraitValues.Add(CreatureTraitId.StrongAndHealthy, 10);

                EnumToAhSkillMapFreedom.Add(CreatureTraitId.CertainSpark, 41);
                EnumToAhSkillMapEpic.Add(CreatureTraitId.CertainSpark, 23.19F);
                DefaultTraitValues.Add(CreatureTraitId.CertainSpark, 0);
            }

            internal static float GetMinimumAhSkillForTraits(CreatureTrait[] traits, bool epicCurve)
            {
                float result = 0;
                foreach (var trait in traits)
                {
                    float skillForThisTrait = GetAhSkillForTrait(trait, epicCurve);

                    if (skillForThisTrait > result) result = skillForThisTrait;
                }
                return result;
            }

            internal static float GetAhSkillForTrait(CreatureTrait creatureTrait, bool epicCurve)
            {
                if (epicCurve) return EnumToAhSkillMapEpic[creatureTrait.CreatureTraitId];
                else return EnumToAhSkillMapFreedom[creatureTrait.CreatureTraitId];
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

            internal static CreatureTrait[] GetMissingTraits(CreatureTrait[] creatureTraits, float inspectSkill, bool epicCurve)
            {
                var correctDict = (epicCurve ? EnumToAhSkillMapEpic : EnumToAhSkillMapFreedom);
                List<CreatureTrait> missingTraits = new List<CreatureTrait>();

                foreach (var keyval in correctDict)
                {
                    if (inspectSkill < keyval.Value && creatureTraits.All(x => x.CreatureTraitId != keyval.Key))
                        missingTraits.Add(new CreatureTrait(keyval.Key));
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

            internal static CreatureTrait[] GetTraitsUpToSkillLevel(float skillLevel, bool isEpic)
            {
                Dictionary<CreatureTraitId, float> correctDict;
                if (isEpic) correctDict = EnumToAhSkillMapEpic;
                else correctDict = EnumToAhSkillMapFreedom;
                List<CreatureTrait> result = new List<CreatureTrait>();
                foreach (var keyval in correctDict)
                {
                    if (keyval.Value < skillLevel) result.Add(new CreatureTrait(keyval.Key));
                }
                return result.ToArray();
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

        internal static float GetMinSkillForTraits(CreatureTrait[] traitlist, bool epicCurve)
        {
            return Helper.GetMinimumAhSkillForTraits(traitlist, epicCurve);
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

        internal static CreatureTrait[] GetMissingTraits(CreatureTrait[] creatureTraits, float inspectSkill, bool epicCurve)
        {
            return Helper.GetMissingTraits(creatureTraits, inspectSkill, epicCurve);
        }

        internal bool IsUnknownForThisCreature(Creature creature)
        {
            return Granger.CreatureTrait.GetSkillForTrait(this, creature.EpicCurve) > creature.TraitsInspectSkill;
        }

        private static float GetSkillForTrait(CreatureTrait creatureTrait, bool epicCurve)
        {
            return Helper.GetAhSkillForTrait(creatureTrait, epicCurve);
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

        internal static float GetFullTraitVisibilityCap(bool epicCurve)
        {
            return Helper.GetFullTraitVisibilityCap(epicCurve);
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

        internal static CreatureTrait[] GetTraitsUpToSkillLevel(float skillLevel, bool isEpic)
        {
            return Helper.GetTraitsUpToSkillLevel(skillLevel, isEpic);
        }
    }
}
