using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.ValuePreset
{
    public class TraitValuator
    {
        public class NoValuesAvailableException : Exception
        {
            public NoValuesAvailableException() : base(){}
            public NoValuesAvailableException(string message):base(message){}
        }

        public const string DEFAULT_id = "default";

        GrangerContext Context;
        FormGrangerMain MainForm;
        public string ValueMapID { get; private set; }

        Dictionary<HorseTrait, int> ValueMap = new Dictionary<HorseTrait, int>();
        bool _usingDefault = false;
        bool _thisValueMapIsNoMore = false;
        /// <summary>
        /// generates new valuator with default hardcoded trait values
        /// </summary>
        public TraitValuator(FormGrangerMain mainForm)
        {
            _usingDefault = true;
        }

        /// <summary>
        /// generates new valuator attempting to use custom values from database,
        /// exception when no values found
        /// </summary>
        /// <param name="valueMapID"></param>
        public TraitValuator(FormGrangerMain mainForm, string valueMapID, GrangerContext context)
        {
            MainForm = mainForm;
            Context = context;
            ValueMapID = valueMapID;
            if (valueMapID == DEFAULT_id) _usingDefault = true;
            else
            {
                RebuildValues();
                context.OnTraitValuesModified += context_OnTraitValuesModified;
            }
        }

        void context_OnTraitValuesModified(object sender, EventArgs e)
        {
            RebuildValues();
        }

        void RebuildValues()
        {
            ValueMap.Clear();
            TraitValueEntity[] traitvalues = Context.TraitValues.Where(x => x.ValueMapID == ValueMapID).ToArray();
            if (traitvalues.Length > 0)
            {
                foreach (var traitvalue in traitvalues)
                { 
                    ValueMap.Add(new HorseTrait(traitvalue.Trait.Trait), traitvalue.Value); 
                }
                var goodtraits = ValueMap.Select(x => x.Value).Where(x => x > 0);
                MaxPossibleValue = goodtraits.Sum();
                var badtraits = ValueMap.Select(x => x.Value).Where(x => x < 0);
                MinPossibleValue = badtraits.Sum();
            }
            else
            {
                _thisValueMapIsNoMore = true;
            }
        }

        public int GetValueForTrait(HorseTrait trait)
        {
            if (_thisValueMapIsNoMore)
            {
                _usingDefault = true;
                MainForm.InvalidateTraitValuator();
            }

            if (_usingDefault) return HorseTrait.GetDefaultValue(trait);

            int result = 0;
            ValueMap.TryGetValue(trait, out result);
            return result;
        }

        internal int Evaluate(HorseTrait[] traits)
        {
            int result = 0;
            foreach (var trait in traits)
            {
                result += GetValueForTrait(trait);
            }
            return result;
        }

        internal int Evaluate(HorseTrait[] traits, bool positiveOnly)
        {
            int result = 0;
            foreach (var trait in traits)
            {
                int val = GetValueForTrait(trait);
                if (positiveOnly && val > 0) result += GetValueForTrait(trait);
                else if (!positiveOnly && val < 0) result += GetValueForTrait(trait);
            }
            return result;
        }

        internal int GetValueForHorse(Horse horse)
        {
            return Evaluate(horse.Traits);
        }

        int GetPotentialValueForHorse(Horse horse, bool positive)
        {
            var missingTraits = HorseTrait.GetMissingTraits(horse.Traits, horse.TraitsInspectSkill, horse.EpicCurve);
            return Evaluate(missingTraits, positive);
        }

        internal int GetPotentialPositiveValueForHorse(Horse horse)
        {
            return GetPotentialValueForHorse(horse, true);
        }

        internal int GetPotentialNegativeValueForHorse(Horse horse)
        {
            return GetPotentialValueForHorse(horse, false);
        }

        public int MaxPossibleValue { get; private set; }

        public int MinPossibleValue { get; private set; }
    }
}
