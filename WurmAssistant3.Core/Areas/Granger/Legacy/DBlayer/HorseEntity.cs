using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    [Table(Name = DBSchema.HorsesTableName)]
    public class HorseEntity
    {
        [Column(Name = "id", IsPrimaryKey = true)]
        public int ID;
        [Column(Name = "herd")]
        public string Herd;

        [Column(Name = "name")]
        public string Name;
        [Column(Name = "fathername")]
        public string FatherName;
        [Column(Name = "mothername")]
        public string MotherName;

        [Column(Name = "traits")]
        string _Traits;

        public List<HorseTrait> Traits
        {
            get
            {
                return HorseTrait.DBHelper.FromStrINTRepresentation(_Traits);
            }
            set
            {
                _Traits = HorseTrait.DBHelper.ToINTStrRepresentation(value);
            }
        }

        [Column(Name = "notinmood")]
        public DateTime? NotInMood;
        [Column(Name = "pregnantuntil")]
        public DateTime? PregnantUntil;
        [Column(Name = "groomedon")]
        public DateTime? GroomedOn;
        [Column(Name = "ismale")]
        public bool? IsMale;
        [Column(Name = "takencareofby")]
        public string TakenCareOfBy;
        [Column(Name = "traitsinspectedatskill")]
        public float? TraitsInspectedAtSkill;
        [Column(Name = "epiccurve")]
        public bool? EpicCurve;
        [Column(Name = "age")]
        string _Age;
        public HorseAge Age
        {
            get { return new HorseAge(_Age); }
            set { _Age = value.ToDBValue(); }
        }
        [Column(Name = "color")]
        string _Color;
        public HorseColor Color
        {
            get { return new HorseColor(_Color); }
            set { _Color = value.ToDBValue(); }
        }
        [Column(Name = "comments")]
        public string Comments;

        [Column(Name = "specialtags")]
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

        [Column(Name = "pairedwith")]
        public int? PairedWith;

        [Column(Name = "brandedfor")]
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

        [Column(Name = "birthdate")]
        public DateTime? BirthDate;
    }
}