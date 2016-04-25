using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    /// <summary>
    /// Represents information parsed from skill gain line.
    /// </summary>
    public class SkillInfo
    {
        public SkillInfo(string name, float value, DateTime stamp, float? gain)
        {
            NameNormalized = WurmSkills.NormalizeSkillName(name);
            Value = value;
            Stamp = stamp;
            Gain = gain;
        }

        /// <summary>
        /// Name of the skill, normalized to uppercase.
        /// </summary>
        public string NameNormalized { get; private set; }

        /// <summary>
        /// Value of the skill.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Stamp of the source log line.
        /// </summary>
        public DateTime Stamp { get; private set; }

        /// <summary>
        /// Skill gain value, if parsed. Null otherwise.
        /// </summary>
        [CanBeNull]
        public float? Gain { get; private set; }

        /// <summary>
        /// Server where this skill info originates from.
        /// Can be null if server could not be established or skill info originates from outside WurmApi.
        /// </summary>
        [CanBeNull]
        public IWurmServer Server { get; internal set; }

        /// <summary>
        /// Checks if this skill entry matches skill name. Case insensitive.
        /// </summary>
        /// <param name="skillName"></param>
        /// <returns></returns>
        public bool IsSkillName(string skillName)
        {
            skillName = skillName.ToUpperInvariant();
            return NameNormalized.Equals(skillName);
        }
    }
}