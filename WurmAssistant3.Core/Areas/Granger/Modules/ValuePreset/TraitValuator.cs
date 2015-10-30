using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.ValuePreset
{
    public class TraitValuator
    {
        public class NoValuesAvailableException : Exception
        {
            public NoValuesAvailableException() : base(){}
            public NoValuesAvailableException(string message):base(message){}
        }

        public const string DefaultId = "default";

        readonly GrangerContext context;
        readonly FormGrangerMain mainForm;
        public string ValueMapId { get; private set; }

        readonly Dictionary<HorseTrait, int> valueMap = new Dictionary<HorseTrait, int>();
        bool usingDefault = false;
        bool thisValueMapIsNoMore = false;
        /// <summary>
        /// generates new valuator with default hardcoded trait values
        /// </summary>
        public TraitValuator(FormGrangerMain mainForm)
        {
            usingDefault = true;
        }

        /// <summary>
        /// generates new valuator attempting to use custom values from database,
        /// exception when no values found
        /// </summary>
        /// <param name="valueMapID"></param>
        public TraitValuator(FormGrangerMain mainForm, string valueMapID, GrangerContext context)
        {
            this.mainForm = mainForm;
            this.context = context;
            ValueMapId = valueMapID;
            if (valueMapID == DefaultId) usingDefault = true;
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
            valueMap.Clear();
            TraitValueEntity[] traitvalues = context.TraitValues.Where(x => x.ValueMapID == ValueMapId).ToArray();
            if (traitvalues.Length > 0)
            {
                foreach (var traitvalue in traitvalues)
                { 
                    valueMap.Add(new HorseTrait(traitvalue.Trait.Trait), traitvalue.Value); 
                }
                var goodtraits = valueMap.Select(x => x.Value).Where(x => x > 0);
                MaxPossibleValue = goodtraits.Sum();
                var badtraits = valueMap.Select(x => x.Value).Where(x => x < 0);
                MinPossibleValue = badtraits.Sum();
            }
            else
            {
                thisValueMapIsNoMore = true;
            }
        }

        public int GetValueForTrait(HorseTrait trait)
        {
            if (thisValueMapIsNoMore)
            {
                usingDefault = true;
                mainForm.InvalidateTraitValuator();
            }

            if (usingDefault) return HorseTrait.GetDefaultValue(trait);

            int result = 0;
            valueMap.TryGetValue(trait, out result);
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
