using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public class TraitValuator
    {
        public const string DefaultId = "default";

        readonly GrangerContext context;
        readonly FormGrangerMain mainForm;

        readonly Dictionary<CreatureTrait, int> valueMap = new Dictionary<CreatureTrait, int>();
        bool usingDefault = false;
        bool thisValueMapIsNoMore = false;

        /// <summary>
        /// Creates new valuator with default hardcoded trait values
        /// </summary>
        public TraitValuator()
        {
            usingDefault = true;
        }

        /// <summary>
        /// Creates new valuator with values loaded from database
        /// </summary>
        public TraitValuator(FormGrangerMain mainForm, string valueMapId, GrangerContext context)
        {
            this.mainForm = mainForm;
            this.context = context;
            ValueMapId = valueMapId;
            if (valueMapId == DefaultId) usingDefault = true;
            else
            {
                RebuildValues();
                context.OnTraitValuesModified += context_OnTraitValuesModified;
            }
        }

        public string ValueMapId { get; private set; }

        void context_OnTraitValuesModified(object sender, EventArgs e)
        {
            RebuildValues();
        }

        void RebuildValues()
        {
            valueMap.Clear();
            TraitValueEntity[] traitvalues = context.TraitValues.Where(x => x.ValueMapId == ValueMapId).ToArray();
            if (traitvalues.Length > 0)
            {
                foreach (var traitvalue in traitvalues)
                { 
                    valueMap.Add(new CreatureTrait(traitvalue.Trait.CreatureTraitId), traitvalue.Value); 
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

        public int MaxPossibleValue { get; private set; }

        public int MinPossibleValue { get; private set; }
    }
}
