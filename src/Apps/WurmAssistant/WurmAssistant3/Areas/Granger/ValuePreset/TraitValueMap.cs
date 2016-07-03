using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    class TraitValueMap
    {
        readonly string traitValueMapId;
        readonly Dictionary<CreatureTraitId, int> valueMap;
        readonly GrangerContext context;

        public TraitValueMap(GrangerContext context, string traitValueMapId)
        {
            this.context = context;
            this.traitValueMapId = traitValueMapId;

            valueMap = CreatureTrait.GetAllDefaultValues();

            if (traitValueMapId == TraitValuator.DefaultId)
            {
                ReadOnly = true;
            }
            else
            {
                ReadOnly = false;
                var entities = this.context.TraitValues.Where(x => x.ValueMapId == traitValueMapId);
                foreach (var entity in entities)
                {
                    valueMap[entity.Trait.CreatureTraitId] = entity.Value;
                }
            }
        }

        public bool ReadOnly { get; private set; }

        public IReadOnlyDictionary<CreatureTraitId, int> ValueMap => valueMap;

        public void ModifyTraitValue(CreatureTraitId creatureTrait, int newValue)
        {
            if (ReadOnly) throw new InvalidOperationException("this list is read-only");

            valueMap[creatureTrait] = newValue;
        }

        /// <summary>
        /// InvalidOperationException if list is read-only
        /// </summary>
        public void Save()
        {
            if (ReadOnly) throw new InvalidOperationException("this list is read-only");

            context.UpdateOrCreateTraitValueMap(valueMap, traitValueMapId);
        }
    }
}
