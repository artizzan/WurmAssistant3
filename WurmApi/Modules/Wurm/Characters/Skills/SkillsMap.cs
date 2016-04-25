using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class SkillsMap
    {
        sealed class CompositeSkillKey : IEquatable<CompositeSkillKey>
        {
            public string SkillNameNormalized { get; }
            public ServerGroup ServerGroupId { get; }

            public CompositeSkillKey(string skillNameNormalized, ServerGroup serverGroupId)
            {
                SkillNameNormalized = skillNameNormalized;
                ServerGroupId = serverGroupId;
            }

            public bool Equals(CompositeSkillKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(SkillNameNormalized, other.SkillNameNormalized) && Equals(ServerGroupId, other.ServerGroupId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is CompositeSkillKey && Equals((CompositeSkillKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SkillNameNormalized?.GetHashCode() ?? 0)*397) ^ (ServerGroupId?.GetHashCode() ?? 0);
                }
            }

            public static bool operator ==(CompositeSkillKey left, CompositeSkillKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(CompositeSkillKey left, CompositeSkillKey right)
            {
                return !Equals(left, right);
            }
        }

        private readonly object locker = new object();
        readonly Dictionary<CompositeSkillKey, SkillInfo> skillsMap = new Dictionary<CompositeSkillKey, SkillInfo>();

        public bool UpdateSkill([NotNull] SkillInfo newSkillInfo, [NotNull] IWurmServer server)
        {
            if (newSkillInfo == null)
                throw new ArgumentNullException(nameof(newSkillInfo));
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            var key = new CompositeSkillKey(newSkillInfo.NameNormalized, server.ServerGroup);
            bool skillUpdated = false;
            lock (locker)
            {
                SkillInfo info;
                if (skillsMap.TryGetValue(key, out info))
                {
                    if (info.Stamp < newSkillInfo.Stamp)
                    {
                        skillsMap[key] = newSkillInfo;
                        skillUpdated = true;
                    }
                }
                else
                {
                    skillsMap.Add(key, newSkillInfo);
                    skillUpdated = true;
                }
            }
            return skillUpdated;
        }

        public SkillInfo TryGetSkill([NotNull] string skillName, ServerGroup serverGroup)
        {
            if (skillName == null)
                throw new ArgumentNullException(nameof(skillName));
            skillName = WurmSkills.NormalizeSkillName(skillName);

            var key = new CompositeSkillKey(skillName, serverGroup);

            lock (locker)
            {
                SkillInfo info;
                if (skillsMap.TryGetValue(key, out info))
                {
                    return info;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}