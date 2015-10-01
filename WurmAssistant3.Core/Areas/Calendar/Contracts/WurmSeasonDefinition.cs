using System;
using AldursLab.Essentials.Debugging;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Calendar.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WurmSeasonDefinition
    {
        [JsonProperty] string seasonName = "";
        [JsonProperty] int dayBegin;
        [JsonProperty] int dayEnd;
        [JsonProperty] Guid id;
        [JsonProperty] bool isDefault;
        [JsonProperty] bool disabled;

        public WurmSeasonDefinition(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        [JsonConstructor]
        public WurmSeasonDefinition()
        {
            id = Guid.NewGuid();
        }

        public string SeasonName
        {
            get { return seasonName; }
            set
            {
                if (value == null) Assertions.ThrowIfDebug();

                seasonName = value ?? string.Empty;
            }
        }

        public int DayBegin
        {
            get { return dayBegin; }
            set
            {
                if (value > WurmCalendar.DaysInYear || value < 0) Assertions.ThrowIfDebug();

                dayBegin = value.ConstrainToRange(0,WurmCalendar.DaysInYear);
            }
        }

        public int DayEnd
        {
            get { return dayEnd; }
            set
            {
                if (value > WurmCalendar.DaysInYear || value < 0) Assertions.ThrowIfDebug();

                dayEnd = value.ConstrainToRange(0, WurmCalendar.DaysInYear);
            }
        }

        public int Length
        {
            get
            {
                int end = DayEnd;
                if (DayBegin > end)
                {
                    end += WurmCalendar.DaysInYear;
                }
                return end - DayBegin;
            }
        }

        public Guid Id
        {
            get { return id; }
            set
            {
                if (value == Guid.Empty) Assertions.ThrowIfDebug();
                id = value;
            }
        }

        public bool IsDefault
        {
            get { return isDefault; }
        }

        public bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        public WurmSeasonDefinition CreateCopy()
        {
            return (WurmSeasonDefinition)this.MemberwiseClone();
        }
    }
}