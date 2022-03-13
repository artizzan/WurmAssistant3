using System;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class Creature
    {
        static readonly System.Drawing.Color? DefaultBestBreedHintColor = (System.Drawing.Color)(new HslColor(120D, 240D, 180D));

        readonly GrangerContext context;
        readonly CreatureColorDefinitions creatureColorDefinitions;
        readonly FormGrangerMain mainForm;

        double? cachedBreedValue = null;

        public Creature(
            FormGrangerMain mainForm, 
            CreatureEntity entity, 
            GrangerContext context,
            [NotNull] CreatureColorDefinitions creatureColorDefinitions)
        {
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
            BreedHintColor = null;
            this.mainForm = mainForm;
            Entity = entity;
            this.context = context;
            this.creatureColorDefinitions = creatureColorDefinitions;
        }

        public CreatureEntity Entity { get; private set; }

        public int Value => mainForm.CurrentValuator.GetValueForCreature(this);

        double? BreedValue 
        { 
            get 
            {
                var breedResults = mainForm.CurrentAdvisor.GetBreedingValue(this);
                if (breedResults.HasValue)
                {
                    if (breedResults.Value.Ignored) return null;
                    else if (breedResults.Value.Discarded) return double.NegativeInfinity;
                    else return breedResults.Value.Value;
                }
                else return null;
            } 
        }

        public double? CachedBreedValue => cachedBreedValue;

        public void RebuildCachedBreedValue()
        {
            cachedBreedValue = BreedValue;
        }

        /// <summary>
        ///  Used to color entire row, null if no color set
        /// </summary>
        public System.Drawing.Color? BreedHintColor { get; private set; }

        /// <summary>
        /// Null if candidate is not best, else contains default best candidate color
        /// </summary>
        public System.Drawing.Color? CreatureBestCandidateColor { get; set; }

        public void RefreshBreedHintColor(double minBreedValue, double maxBreedValue)
        {
            if (CachedBreedValue == maxBreedValue)
            {
                CreatureBestCandidateColor = DefaultBestBreedHintColor;
            }

            if (mainForm.SelectedSingleCreature != null)
            {
                BreedHintColor = mainForm.CurrentAdvisor.GetHintColor(this, minBreedValue, maxBreedValue);
            }
        }

        public void ClearColorHints()
        {
            BreedHintColor = null;
            CreatureBestCandidateColor = null;
        }

        public System.Drawing.Color? CreatureColorBkColor
        {
            get
            {
                CreatureColor hcolor = Color;
                if (hcolor == CreatureColor.GetDefaultColor()) return null;
                else return hcolor.SystemDrawingColor;
            }
        }

        public CreatureTrait[] Traits
        {
            get { return Entity.Traits.ToArray(); }
            set
            {
                Entity.Traits = value.ToList();
            }
        }

        public override string ToString()
        {
            return Entity.ToString();
        }

        public CreatureColor Color
        {
            get { return creatureColorDefinitions.GetForId(Entity.CreatureColorId); }
            set
            {
                Entity.CreatureColorId = value.CreatureColorId;
            }
        }

        public Creature GetMate()
        {
            if (!HasMate()) return null;

            var mate =
                context.Creatures.Where(x => x.Id == this.Entity.PairedWith)
                       .Select(x => new Creature(mainForm, x, context, creatureColorDefinitions))
                       .ToArray();
            if (mate.Length == 1) 
                return mate.First();
            else if (mate.Length == 0) 
                return null;
            else throw new Exception("duplicate creatures found?");
        }

        public bool HasMate()
        {
            if (this.Entity.PairedWith == null) return false;
            else return true;
        }

        public void SetMate(Creature value)
        {
            if (value == null)
            {
                var mate = GetMate();
                if (mate != null) mate.Entity.PairedWith = null;
                this.Entity.PairedWith = null;
            }
            else
            {
                this.Entity.PairedWith = value.Entity.Id;
                value.Entity.PairedWith = this.Entity.Id;
            }
        }

        public bool IsMale
        {
            get { return (Entity.IsMale ?? false); }
            set { Entity.IsMale = value; }
        }

        public DateTime NotInMoodUntil
        {
            get { return (Entity.NotInMood ?? DateTime.MinValue); }
            set { Entity.NotInMood = value; }
        }

        public DateTime GroomedOn
        {
            get { return (Entity.GroomedOn ?? DateTime.MinValue); }
            set { Entity.GroomedOn = value; }
        }

        public DateTime PregnantUntil
        {
            get { return (Entity.PregnantUntil ?? DateTime.MinValue); }
            set { Entity.PregnantUntil = value; }
        }

        public DateTime BirthDate
        {
            get { return (Entity.BirthDate ?? DateTime.MinValue); }
            set { Entity.BirthDate = value; }
        }

        public bool CheckTag(string name)
        {
            return Entity.CheckTag(name);
        }

        public void SetTag(string name, bool state)
        {
            Entity.SetTag(name, state);
        }

        public float TraitsInspectSkill
        {
            get { return Entity.TraitsInspectedAtSkill ?? 0; }
            set { Entity.TraitsInspectedAtSkill = value; }
        }

        public CreatureAge Age
        {
            get { return Entity.Age; }
            set { Entity.Age = value; }
        }

        public string Name { get { return Entity.Name; } set { Entity.Name = value; } }

        public string InnerName => GetBrandingNameInfo(Entity.Name).InnerName;

        public string Father { get { return Entity.FatherName; } set { Entity.FatherName = value; } }

        public string Mother { get { return Entity.MotherName; } set { Entity.MotherName = value; } }

        public string TakenCareOfBy { get { return Entity.TakenCareOfBy; } set { Entity.TakenCareOfBy = value; } }

        public string BrandedFor { get { return Entity.BrandedFor; } set { Entity.BrandedFor = value; } }

        public string Comments { get { return Entity.Comments; } set { Entity.Comments = value; } }

        public string Herd
        {
            get { return Entity.Herd; }
            set
            {
                if (this.Herd == value) return;

                var targetHerd =
                    context.Creatures.Where(x => x.Herd == value)
                           .Select(x => new Creature(mainForm, x, context, creatureColorDefinitions));

                foreach (var creature in targetHerd)
                {
                    if (creature.IsNotUniquelyIdentifiableWhenComparedTo(this))
                    {
                        throw new Exception("can not change herd because nonunique creatures already exists in target herd");
                    }
                }

                Entity.Herd = value;
            }
        }

        [CanBeNull]
        public string ServerName
        {
            get { return Entity.ServerName; }
            set { Entity.ServerName = value ?? string.Empty; }
        }

        public bool IsNotUniquelyIdentifiableWhenComparedTo(Creature other)
        {
            return !this.Entity.IsUniquelyIdentifiableWhenComparedTo(other.Entity);
        }

        public string HerdAspect => Entity.Herd;
        public string NameAspect => Entity.Name;
        public string FatherAspect => Entity.FatherName;
        public string MotherAspect => Entity.MotherName;
        public string TraitsAspect => CreatureTrait.GetShortString(Entity.Traits.ToArray(), mainForm.CurrentValuator);

        public TimeSpan NotInMoodForAspect
        {
            get
            {
                if (!Entity.NotInMood.HasValue) return TimeSpan.MinValue;
                else
                {
                    return Entity.NotInMood.Value - DateTime.Now;
                }
            }
        }

        public TimeSpan PregnantForAspect
        {
            get
            {
                if (!Entity.PregnantUntil.HasValue) return TimeSpan.MinValue;
                else
                {
                    return Entity.PregnantUntil.Value - DateTime.Now;
                }
            }
        }

        public TimeSpan GroomedAgoAspect
        {
            get
            {
                if (!Entity.GroomedOn.HasValue) return TimeSpan.MaxValue;
                else
                {
                    return DateTime.Now - Entity.GroomedOn.Value;
                }
            }
        }

        public DateTime BirthDateAspect => Entity.BirthDate ?? DateTime.MinValue;

        public TimeSpan ExactAgeAspect => !Entity.BirthDate.HasValue ? TimeSpan.MaxValue : DateTime.Now - Entity.BirthDate.Value;

        public string GenderAspect => Entity.GenderAspect;

        public string TakenCareOfByAspect => Entity.TakenCareOfBy;


        public TraitsInspectedContainer TraitsInspectedAtSkillAspect
        {
            get
            {
                float val = Entity.TraitsInspectedAtSkill ?? 0;
                return new TraitsInspectedContainer() 
                { 
                    Skill = val
                };
            }
        }


        public CreatureAge AgeAspect => this.Age;

        public string ColorAspect
            => Entity.CreatureColorId == DefaultCreatureColorIds.Unknown ? string.Empty : Color.ToString();

        public string TagsAspect { get { return string.Join(", ", Entity.SpecialTags.OrderBy(x => x)); } }

        public string CommentsAspect => Entity.Comments;

        public int ValueAspect => this.Value;

        public double? BreedValueAspect => this.CachedBreedValue;

        public string PairedWithAspect
        {
            get
            {
                Creature mate = GetMate();
                if (mate == null) return string.Empty;
                return mate.ToString();
            }
        }

        public string BrandedForAspect => Entity.BrandedFor ?? string.Empty;

        public string ServerAspect => Entity.ServerName ?? "-Unknown-";

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            Creature p = obj as Creature;
            if ((System.Object)p == null)
            {
                return false;
            }
            
            return this.Entity.Id == p.Entity.Id;
        }

        public bool Equals(Creature p)
        {
            if ((object)p == null)
            {
                return false;
            }
            
            return this.Entity.Id == p.Entity.Id;
        }

        public override int GetHashCode()
        {
            return this.Entity.Id;
        }

        public static bool operator ==(Creature a, Creature b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            
            return a.Equals(b);
        }

        public static bool operator !=(Creature a, Creature b)
        {
            return !(a == b);
        }

        public bool NotInMood => this.NotInMoodUntil > DateTime.Now;

        public bool Pregnant => this.PregnantUntil > DateTime.Now;

        public bool PregnantInLast24H => this.PregnantUntil > DateTime.Now - TimeSpan.FromHours(24);

        internal bool IsInbreedWith(Creature otherCreature)
        {
            if (   Name == otherCreature.Mother
                || Name == otherCreature.Father
                || Mother == otherCreature.Name
                || Father == otherCreature.Name
                || (!string.IsNullOrEmpty(Mother) && Mother == otherCreature.Mother)
                || (!string.IsNullOrEmpty(Father) && Father == otherCreature.Father))
                return true;
            else return false;
        }

        internal bool IsFoal()
        {
            return Age.CreatureAgeId == CreatureAgeId.YoungFoal || Age.CreatureAgeId == CreatureAgeId.AdolescentFoal;
        }

        public static InnerCreatureNameInfo GetBrandingNameInfo(string name)
        {
            var match = Regex.Match(name, @"'(.+)'", RegexOptions.Compiled);
            if (match.Success)
            {
                return new InnerCreatureNameInfo()
                {
                    HasInnerName = true,
                    InnerName = match.Groups[1].Value
                };
            }
            else
            {
                return new InnerCreatureNameInfo()
                {
                    HasInnerName = false,
                    InnerName = string.Empty
                };
            }
        }

        public DateTime SmilexamineLastDate
        {
            get { return (Entity.SmilexamineLastDate ?? DateTime.MinValue); }
            set { Entity.SmilexamineLastDate = value; }
        }
    }
}
