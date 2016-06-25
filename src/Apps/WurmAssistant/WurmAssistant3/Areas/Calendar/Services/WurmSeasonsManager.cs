using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Calendar.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Calendar.Services
{
    [KernelBind(BindingHint.Singleton), PersistentObject("Calendar_WurmSeasonsManager")]
    public class WurmSeasonsManager : PersistentObjectBase
    {
        List<WurmSeasonDefinition> defaultDefinitions = new List<WurmSeasonDefinition>();

        [JsonProperty] 
        readonly List<WurmSeasonDefinition> definitions = new List<WurmSeasonDefinition>();

        public WurmSeasonsManager()
        {
            SetDefaultDefinition(new Guid("87379cd0-c68e-41ff-a260-e3988abcaa9a"), "Oleander", 85, 91);
            SetDefaultDefinition(new Guid("0d141288-56ee-4a06-b563-4af212fba564"), "Maple", 113, 119);
            SetDefaultDefinition(new Guid("7770861c-b165-4bb4-a659-133a75ac2fa3"), "Rose", 113, 140);
            SetDefaultDefinition(new Guid("2df231f1-767d-4335-b42a-1e511e0d2e46"), "Rose", 141, 147);
            SetDefaultDefinition(new Guid("b869075e-2d87-42f9-bf32-6c81707d31e7"), "Lavender", 113, 119);
            SetDefaultDefinition(new Guid("e745ca76-408d-4bcd-b3d6-3b854cb83b2f"), "Lavender", 141, 147);
            SetDefaultDefinition(new Guid("00a0db71-2f4f-4c1f-8073-e204af48f6f1"), "Camellia", 113, 126);
            SetDefaultDefinition(new Guid("fbb72843-8a67-45ae-a64e-943f23e6f40e"), "Cherry", 176, 196);
            SetDefaultDefinition(new Guid("d76ccabf-7783-4224-b842-d8370ff105a0"), "Olive", 204, 224);
            SetDefaultDefinition(new Guid("71bac263-9222-46cc-8945-fa687cbd2c77"), "Olive", 92, 112);
            SetDefaultDefinition(new Guid("8c3f15af-9c4c-4bcb-8068-74d92f20d342"), "Grape", 225, 252);
            SetDefaultDefinition(new Guid("def13950-ade7-4a1b-aa5a-3336940b342e"), "Apple", 225, 252);
            SetDefaultDefinition(new Guid("d34569fe-d126-45ea-8d31-4df6e1bcb517"), "Lemon", 288, 308);
            SetDefaultDefinition(new Guid("39f35042-a302-4b47-8ef8-c6eb6dd61a9e"), "Walnut", 258, 261);
            SetDefaultDefinition(new Guid("ee0fe83f-d934-4d8f-89ae-2837a0c17d03"), "Chestnut", 42, 46);
        }

        protected override void OnPersistentDataLoaded()
        {
            PopulateDefaults();
        }

        void PopulateDefaults()
        {
            foreach (var wurmSeasonDefinition in defaultDefinitions)
            {
                AddIfNotExists(wurmSeasonDefinition);
            }
        }

        void SetDefaultDefinition(Guid id, [NotNull] string name, int dayBegin, int dayEnd)
        {
            var def = new WurmSeasonDefinition(true)
            {
                Id = id,
                SeasonName = name,
                DayBegin = dayBegin,
                DayEnd = dayEnd
            };

            defaultDefinitions.Add(def);
        }

        void AddIfNotExists(WurmSeasonDefinition wurmSeasonDefinition)
        {
            if (!definitions.Exists(definition => definition.Id == wurmSeasonDefinition.Id))
            {
                definitions.Add(wurmSeasonDefinition);
            }
        }

        public IEnumerable<WurmSeasonDefinition> AllNotDisabled
        {
            get { return definitions.Where(definition => !definition.Disabled); }
        }

        public IEnumerable<WurmSeasonDefinition> All
        {
            get { return definitions; }
        }

        public event EventHandler<EventArgs> DataChanged;

        public void ReplaceReasons(IEnumerable<WurmSeasonDefinition> seasons)
        {
            definitions.Clear();
            definitions.AddRange(seasons);
            RequestSave();
            OnDataChanged();
        }

        public IEnumerable<WurmSeasonDefinition> GetDefaults()
        {
            return defaultDefinitions.Select(definition => definition.CreateCopy());
        }

        protected virtual void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
