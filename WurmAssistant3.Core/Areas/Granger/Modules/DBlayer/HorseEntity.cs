using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HorseEntity
    {
        //primary key
        [JsonProperty("id")]
        public int ID;
        [JsonProperty("herd")]
        public string Herd;

        [JsonProperty("name")]
        public string Name;
        [JsonProperty("fathername")]
        public string FatherName;
        [JsonProperty("mothername")]
        public string MotherName;

        [JsonProperty("traits")]
        string _Traits;

        public List<HorseTrait> Traits
        {
            get
            {
                return HorseTrait.DbHelper.FromStrIntRepresentation(_Traits);
            }
            set
            {
                _Traits = HorseTrait.DbHelper.ToIntStrRepresentation(value);
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
        string _Age;
        public HorseAge Age
        {
            get { return new HorseAge(_Age); }
            set { _Age = value.ToDbValue(); }
        }
        [JsonProperty("color")]
        string _Color;
        public HorseColor Color
        {
            get { return new HorseColor(_Color); }
            set { _Color = value.ToDbValue(); }
        }
        [JsonProperty("comments")]
        public string Comments;

        [JsonProperty("specialtags")]
        string _SpecialTags;
        public HashSet<string> SpecialTags
        {
            get
            {
                var result = new HashSet<string>();
                if (_SpecialTags == null) return result;
                foreach (var tag in _SpecialTags.Split(','))
                {
                    if (!string.IsNullOrEmpty(tag)) result.Add(tag);
                }
                return result;
            }
            set
            {
                _SpecialTags = string.Join(",", value);
            }
        }

        public string SpecialTagsRaw
        {
            get { return _SpecialTags; }
            set { _SpecialTags = value; }
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

        static public int GenerateNewHorseID(GrangerContext context)
        {
            try
            {
                return context.Horses.Max(x => x.ID) + 1;
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

        public bool IsDifferentIdentityThan(HorseEntity otherHorse)
        {
            return this.Name != otherHorse.Name;
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