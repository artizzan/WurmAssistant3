using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;

namespace AldursLab.WurmApi
{
    public interface IWurmCharacterSkills
    {
        /// <summary>
        /// Attempts to find out current level of a given skill.
        /// Combined data of live logs, logs history and skill dumps is used to obtain this information.
        /// </summary>
        /// <param name="skillName">Name of the skill, case insensitive.</param>
        /// <param name="serverGroup"></param>
        /// <param name="maxTimeToLookBackInLogs">Maximum number of days to scan logs history, before giving up.</param>
        /// <returns></returns>
        Task<SkillInfo> TryGetCurrentSkillLevelAsync(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs);

        /// <summary>
        /// Attempts to find out current level of a given skill.
        /// Combined data of live logs, logs history and skill dumps is used to obtain this information.
        /// </summary>
        /// <param name="skillName">Name of the skill, case insensitive.</param>
        /// <param name="serverGroup"></param>
        /// <param name="maxTimeToLookBackInLogs">Maximum number of days to scan logs history, before giving up.</param>
        /// <returns></returns>
        SkillInfo TryGetCurrentSkillLevel(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs);

        /// <summary>
        /// Triggered, when some skills have changed since last invocation of this event.
        /// Only live logs feed is being monitored.
        /// </summary>
        event EventHandler<SkillsChangedEventArgs> SkillsChanged;

        /// <summary>
        /// Obtains latest skill group for specific server group.
        /// </summary>
        /// <param name="serverGroupId"></param>
        /// <returns></returns>
        Task<SkillDump> GetLatestSkillDumpAsync(string serverGroupId);
    }

    public class SkillsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// List of skill changes since last invocation of this event.
        /// List is ordered ascending by skill info timestamp (oldest first).
        /// Names of the skills are always normalized to uppercase.
        /// </summary>
        public IReadOnlyList<SkillInfo> SkillChanges { get; private set; }

        public SkillsChangedEventArgs(SkillInfo[] skillChanges)
        {
            SkillChanges = skillChanges;
        }

        /// <summary>
        /// True if skill has changed.
        /// </summary>
        /// <param name="skillName">Case insensitive.</param>
        /// <returns></returns>
        public bool HasSkillChanged(string skillName)
        {
            return SkillChanges.Any(s => s.IsSkillName(skillName));
        }
    }
}
