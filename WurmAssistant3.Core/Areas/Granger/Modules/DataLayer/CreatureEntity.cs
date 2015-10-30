using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureEntity
    {
        //primary key
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("herd")]
        public string Herd;

        [JsonProperty("name")]
        public string Name;
        [JsonProperty("fathername")]
        public string FatherName;
        [JsonProperty("mothername")]
        public string MotherName;

        [JsonProperty("traits")]
        string traits;

        public List<CreatureTrait> Traits
        {
            get
            {
                return CreatureTrait.DbHelper.FromStrIntRepresentation(traits);
            }
            set
            {
                traits = CreatureTrait.DbHelper.ToIntStrRepresentation(value);
            }
        }

        [JsonProperty("notinmood")]
        public DateTime? NotInMood;
        [JsonProperty("pregnantuntil")]
        public DateTime? PregnantUntil;
        [JsonProperty("groomedon")]
        public DateTime? GroomedOn;
        [JsonProperty("ismale")]
        public bool? IsMale;
        [JsonProperty("takencareofby")]
        public string TakenCareOfBy;
        [JsonProperty("traitsinspectedatskill")]
        public float? TraitsInspectedAtSkill;
        [JsonProperty("epiccurve")]
        public bool? EpicCurve;
        [JsonProperty("age")]
        string age;
        public CreatureAge Age
        {
            get { return new CreatureAge(age); }
            set { age = value.ToDbValue(); }
        }
        [JsonProperty("color")]
        string color;
        public CreatureColor Color
        {
            get { return new CreatureColor(color); }
            set { color = value.ToDbValue(); }
        }
        [JsonProperty("comments")]
        public string Comments;

        [JsonProperty("specialtags")]
        string specialTags;
        public HashSet<string> SpecialTags
        {
            get
            {
                var result = new HashSet<string>();
                if (specialTags == null) return result;
                foreach (var tag in specialTags.Split(','))
                {
                    if (!string.IsNullOrEmpty(tag)) result.Add(tag);
                }
                return result;
            }
            set
            {
                specialTags = string.Join(",", value);
            }
        }

        [JsonProperty("serverName"), CanBeNull]
        public string ServerName { get; set; }

        public string SpecialTagsRaw
        {
            get { return specialTags; }
            set { specialTags = value; }
        }

        public bool CheckTag(string name)
        {
            return SpecialTags.Contains(name);
        }

        public void SetTag(string name, bool state)
        {
            var tags = SpecialTags;
            if (state) tags.Add(name); else tags.Remove(name);
            SpecialTags = tags;
        }

        public enum SecondaryInfoTag { None, Diseased, Fat, Starving }

        public SecondaryInfoTag SecondaryInfoTagSetter
        {
            set { SetSecondaryInfoTag(value);}
        }

        public void SetSecondaryInfoTag(SecondaryInfoTag name)
        {
            SetTag("diseased", name == SecondaryInfoTag.Diseased);
            SetTag("fat", name == SecondaryInfoTag.Fat);
            SetTag("starving", name == SecondaryInfoTag.Starving);
        }

        public void SetTagDead()
        {
            var tags = SpecialTags;
            tags.Add("dead");
            SpecialTags = tags;
        }

        [JsonProperty("pairedwith")]
        public int? PairedWith;

        [JsonProperty("brandedfor")]
        public string BrandedFor;

        static public int GenerateNewCreatureId(GrangerContext context)
        {
            try
            {
                return context.Creatures.Max(x => x.Id) + 1;
            }
            catch (InvalidOperationException)
            {
                return 1;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", Name, GenderAspect, Herd);
        }

        public bool IsDifferentIdentityThan(CreatureEntity otherCreature)
        {
            return this.Name != otherCreature.Name;
        }

        public string GenderAspect
        {
            get
            {
                if (!IsMale.HasValue)
                {
                    return "???";
                }
                else
                {
                    if (IsMale.Value) return "male"; else return "female";
                }
            }
        }

        [JsonProperty("birthdate")]
        public DateTime? BirthDate;
    }
}