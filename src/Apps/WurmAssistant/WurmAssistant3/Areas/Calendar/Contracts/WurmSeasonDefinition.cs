using System;
using System.Diagnostics;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Calendar.Contracts
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
                Debug.Assert(value != null);

                seasonName = value ?? string.Empty;
            }
        }

        public int DayBegin
        {
            get { return dayBegin; }
            set
            {
                Debug.Assert(value > 0 && value < WurmCalendar.DaysInYear);

                dayBegin = value.ConstrainToRange(0,WurmCalendar.DaysInYear);
            }
        }

        public int DayEnd
        {
            get { return dayEnd; }
            set
            {
                Debug.Assert(value > 0 && value < WurmCalendar.DaysInYear);

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
                Debug.Assert(value != Guid.Empty);
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