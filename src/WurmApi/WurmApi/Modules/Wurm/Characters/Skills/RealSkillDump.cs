using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class RealSkillDump : SkillDump
    {
        readonly SkillDumpInfo dumpInfo;
        readonly IReadOnlyDictionary<string, float> skillLevels;
        readonly IWurmApiLogger logger;

        public RealSkillDump(ServerGroup serverGroup, [NotNull] SkillDumpInfo dumpInfo,
            [NotNull] IWurmApiLogger logger)
            : base(serverGroup)
        {
            if (dumpInfo == null) throw new ArgumentNullException(nameof(dumpInfo));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.dumpInfo = dumpInfo;
            this.logger = logger;
            skillLevels = ParseDump();
        }

        Dictionary<string, float> ParseDump()
        {
            var fileLines = File.ReadAllLines(dumpInfo.FileInfo.FullName);
            Dictionary<string, float> skills = new Dictionary<string, float>();
            var parser = new SkillEntryParser(logger);
            foreach (var line in fileLines)
            {
                if (line.StartsWith("Skills")
                    || line.StartsWith("Player:") 
                    || line.StartsWith("Premium:") 
                    || line.StartsWith("Server:") 
                    || line.StartsWith("Characteristics") 
                    || line.StartsWith("Religion") 
                    || line.StartsWith("-----"))
                {
                    continue;
                }
                var match = Regex.Match(line, @"(.+): (.+) .+ .+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
                var skillName = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(skillName))
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Unparseable skill name in dump file {0}, raw line: {1}",
                            dumpInfo.FileInfo.FullName,
                            line),
                        this,
                        null);
                    continue;
                }
                var level = parser.TryParseFloatInvariant(match.Groups[2].Value);
                if (level == null)
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Unparseable skill value in dump file {0}, raw line: {1}",
                            dumpInfo.FileInfo.FullName,
                            line),
                        this,
                        null);
                    continue;
                }
                skills[WurmSkills.NormalizeSkillName(skillName)] = level.Value;
            }
            return skills;
        }

        public override float? TryGetSkillLevel(string skillName)
        {
            float value;
            if (skillLevels.TryGetValue(WurmSkills.NormalizeSkillName(skillName), out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public override DateTime Stamp => dumpInfo.Stamp;

        public override IEnumerable<SkillInfo> All
        {
            get { return skillLevels.Select(pair => new SkillInfo(pair.Key, pair.Value, Stamp, null)); }
        }

        public override bool IsNull => false;
    }
}