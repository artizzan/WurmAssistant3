using System;
using System.Collections.Generic;

namespace WurmAssistantDataTransfer.Dtos
{
    public class Creature
    {
        public Creature()
        {
            CreatureTraits = new List<string>();
            SpecialTags = new List<string>();
        }

        public Guid? GlobalId { get; set; }
        public int LocalId { get; set; }

        public Guid? HerdGlobalId { get; set; }
        public string HerdId { get; set; }

        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public List<string> CreatureTraits { get; set; }
        public DateTime? NotInMood { get; set; }
        public DateTime? PregnantUntil { get; set; }
        public DateTime? GroomedOn { get; set; }
        public bool? IsMale { get; set; }
        public string TakenCareOfBy { get; set; }
        public float? TraitsInspectedAtSkill { get; set; }
        public bool? EpicCurve { get; set; }
        public string CreatureAge { get; set; }
        public string CreatureColor { get; set; }
        public string Comments { get; set; }
        public List<string> SpecialTags { get; set; }
        public string ServerName { get; set; }
        public int? PairedWith { get; set; }
        public string BrandedFor { get; set; }
        public DateTime? BirthDate { get; set; }

        public override string ToString()
        {
            return string.Format("GlobalId: {0}, HerdId: {1}, Name: {2}, IsMale: {3}", GlobalId, HerdId, Name, IsMale);
        }
    }
}