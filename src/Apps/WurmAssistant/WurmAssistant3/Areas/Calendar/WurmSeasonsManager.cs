using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    [KernelBind(BindingHint.Singleton), PersistentObject("Calendar_WurmSeasonsManager")]
    public class WurmSeasonsManager : PersistentObjectBase
    {
        List<WurmSeasonDefinition> defaultDefinitions = new List<WurmSeasonDefinition>();

        [JsonProperty] 
        readonly List<WurmSeasonDefinition> definitions = new List<WurmSeasonDefinition>();

        public WurmSeasonsManager()
        {
            SetDefaultDefinition(new Guid("87379cd0-c68e-41ff-a260-e3988abcaa9a"), "Oleander",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Leaf, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Leaf, 2, 7));
            SetDefaultDefinition(new Guid("0d141288-56ee-4a06-b563-4af212fba564"), "Maple",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 4, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 4, 7));
            SetDefaultDefinition(new Guid("7770861c-b165-4bb4-a659-133a75ac2fa3"), "Rose",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 3, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 3, 7));
            SetDefaultDefinition(new Guid("b869075e-2d87-42f9-bf32-6c81707d31e7"), "Lavender",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Bear, 2, 7));
            SetDefaultDefinition(new Guid("00a0db71-2f4f-4c1f-8073-e204af48f6f1"), "Camellia",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Leaf, 4, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Leaf, 4, 7));
            SetDefaultDefinition(new Guid("fbb72843-8a67-45ae-a64e-943f23e6f40e"), "Cherry",
                WurmDateTime.CalculateDayInYear(WurmStarfall.WhiteShark, 1, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.WhiteShark, 1, 7));
            SetDefaultDefinition(new Guid("d76ccabf-7783-4224-b842-d8370ff105a0"), "Olive",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 1, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 1, 7));
            SetDefaultDefinition(new Guid("8c3f15af-9c4c-4bcb-8068-74d92f20d342"), "Grape",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 1, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 1, 7));
            SetDefaultDefinition(new Guid("def13950-ade7-4a1b-aa5a-3336940b342e"), "Apple",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 3, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 3, 7));
            SetDefaultDefinition(new Guid("d34569fe-d126-45ea-8d31-4df6e1bcb517"), "Lemon",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 2, 7));
            SetDefaultDefinition(new Guid("39f35042-a302-4b47-8ef8-c6eb6dd61a9e"), "Walnut",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 2, 7));
            SetDefaultDefinition(new Guid("ee0fe83f-d934-4d8f-89ae-2837a0c17d03"), "Chestnut",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 4, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Raven, 4, 7));
            SetDefaultDefinition(new Guid("330590fe-dba1-41b5-b567-4cad9fc8bca7"), "Pinenut",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Diamond, 1, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Diamond, 1, 7));
            SetDefaultDefinition(new Guid("f15f9fbd-724c-42e0-8c81-bb059e43ed4e"), "Acorn",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Snake, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Snake, 2, 7));
            SetDefaultDefinition(new Guid("5129cef2-a405-4a6e-88b6-ac138d812639"), "Blueberry",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 2, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 2, 7));
            SetDefaultDefinition(new Guid("1fd220d5-a1ee-488b-a0b8-102d8c3f9a83"), "Hops",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 3, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 3, 7));
            SetDefaultDefinition(new Guid("b019bcd6-b5bb-4ea9-b06c-c498ba79154a"), "Orange",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 4, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Fire, 4, 7));
            SetDefaultDefinition(new Guid("9cd323e1-ee86-4089-84ab-96ab37c4424a"), "Raspberry",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 1, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 1, 7));
            SetDefaultDefinition(new Guid("8021a7cf-e82e-4889-a0d8-ea1eb2f0926f"), "Hazelnut",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 3, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 3, 7));
            SetDefaultDefinition(new Guid("cfa626e2-e688-46d2-b935-eb095ff6bcca"), "Lingonberry",
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 4, 1),
                WurmDateTime.CalculateDayInYear(WurmStarfall.Dancer, 4, 7));
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
