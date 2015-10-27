using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.ValuePreset
{
    class TraitValueMap
    {
        string TraitValueMapID;
        public Dictionary<HorseTrait.TraitEnum, int> ValueMap;
        GrangerContext Context;

        public bool ReadOnly {get;private set;}

        public TraitValueMap(GrangerContext context, string traitValueMapID)
        {
            Context = context;
            TraitValueMapID = traitValueMapID;

            ValueMap = HorseTrait.GetAllDefaultValues();

            if (traitValueMapID == TraitValuator.DEFAULT_id)
            {
                ReadOnly = true;
            }
            else
            {
                ReadOnly = false;
                var entities = Context.TraitValues.Where(x => x.ValueMapID == traitValueMapID);
                foreach (var entity in entities)
                {
                    ValueMap[entity.Trait.Trait] = entity.Value;
                }
            }
        }

        public void ModifyTraitValue(HorseTrait.TraitEnum trait, int newValue)
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

            Context.UpdateOrCreateTraitValueMap(ValueMap, TraitValueMapID);
        }
    }
}
