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

        readonly Dictionary<CreatureTrait, int> valueMap = new Dictionary<CreatureTrait, int>();
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
                    valueMap.Add(new CreatureTrait(traitvalue.Trait.Trait), traitvalue.Value); 
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

        public int GetValueForTrait(CreatureTrait trait)
        {
            if (thisValueMapIsNoMore)
            {
                usingDefault = true;
                mainForm.InvalidateTraitValuator();
            }

            if (usingDefault) return CreatureTrait.GetDefaultValue(trait);

            int result = 0;
            valueMap.TryGetValue(trait, out result);
            return result;
        }

        internal int Evaluate(CreatureTrait[] traits)
        {
            int result = 0;
            foreach (var trait in traits)
            {
                result += GetValueForTrait(trait);
            }
            return result;
        }

        internal int Evaluate(CreatureTrait[] traits, bool positiveOnly)
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

        internal int GetValueForCreature(Creature creature)
        {
            return Evaluate(creature.Traits);
        }

        int GetPotentialValueForCreature(Creature creature, bool positive)
        {
            var missingTraits = CreatureTrait.GetMissingTraits(creature.Traits, creature.TraitsInspectSkill, creature.EpicCurve);
            return Evaluate(missingTraits, positive);
        }

        internal int GetPotentialPositiveValueForCreature(Creature creature)
        {
            return GetPotentialValueForCreature(creature, true);
        }

        internal int GetPotentialNegativeValueForCreature(Creature creature)
        {
            return GetPotentialValueForCreature(creature, false);
        }

        public int MaxPossibleValue { get; private set; }

        public int MinPossibleValue { get; private set; }
    }
}
