using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    class TraitValueMap
    {
        readonly string traitValueMapId;
        public readonly Dictionary<CreatureTrait.TraitEnum, int> ValueMap;
        readonly GrangerContext context;

        public bool ReadOnly {get;private set;}

        public TraitValueMap(GrangerContext context, string traitValueMapId)
        {
            this.context = context;
            this.traitValueMapId = traitValueMapId;

            ValueMap = CreatureTrait.GetAllDefaultValues();

            if (traitValueMapId == TraitValuator.DefaultId)
            {
                ReadOnly = true;
            }
            else
            {
                ReadOnly = false;
                var entities = this.context.TraitValues.Where(x => x.ValueMapID == traitValueMapId);
                foreach (var entity in entities)
                {
                    ValueMap[entity.Trait.Trait] = entity.Value;
                }
            }
        }

        public void ModifyTraitValue(CreatureTrait.TraitEnum trait, int newValue)
        {
            if (ReadOnly) throw new InvalidOperationException("this list is read-only");

            ValueMap[trait] = newValue;
        }

        /// <summary>
        /// InvalidOperationException if list is read-only
        /// </summary>
        public void Save()
        {
            if (ReadOnly) throw new InvalidOperationException("this list is read-only");

            context.UpdateOrCreateTraitValueMap(ValueMap, traitValueMapId);
        }
    }
}
