using System;
using System.Collections.Generic;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;

namespace AldursLab.WurmApi
{
    public abstract class SkillDump
    {
        protected SkillDump(ServerGroup serverGroup)
        {
            ServerGroupId = serverGroup;
        }

        public ServerGroup ServerGroupId { get; private set; }

        public abstract float? TryGetSkillLevel(string skillName);

        public abstract DateTime Stamp { get; }

        public abstract IEnumerable<SkillInfo> All { get; }

        /// <summary>
        /// Determines, if this skill dump represents actual skill dump.
        /// </summary>
        public abstract bool IsNull { get; }
    }
}