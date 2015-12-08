using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters.Skills;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Modules
{
    class CombatResultsProcessor
    {
        readonly CombatStatus combatStatus;
        readonly ILogger logger;
        readonly EntryMatchingRuleEngine matcher = new EntryMatchingRuleEngine();
        readonly DamageEntryParser damageEntryParser;
        readonly SkillEntryParser skillEntryParser;

        string currentTargetName = string.Empty;
        string CharacterName { get { return combatStatus.CharacterName; } }

        StampedValue<float> lastSkillGain;
        StampedValue<string> lastKill;

        public CombatResultsProcessor(CombatStatus combatStatus, ILogger logger, IWurmApi wurmApi)
        {
            if (combatStatus == null) throw new ArgumentNullException("combatStatus");
            if (logger == null) throw new ArgumentNullException("logger");
            this.combatStatus = combatStatus;
            this.logger = logger;

            damageEntryParser = new DamageEntryParser(logger);
            skillEntryParser = new SkillEntryParser(wurmApi);

            SetupRules();
        }

        void SetupRules()
        {
            SetupCurrentTargetRecognition();
            SetupMissCounts();
            SetupShieldBlocks();
            SetupParryCounts();
            SetupGlancingCounts();
            SetupEvadeCounts();
            SetupTargetPreference();
            SetupDamageCounts();
            SetupFocusLevels();
            SetupIncomingAttackers();
            SetupSpellAttacks();
            SetupKillCounts();
            SetupSkillGains();
        }

        void SetupCurrentTargetRecognition()
        {
            // does not log for other characters.

            // You try to maul a Aged troll.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You try to \w+ a (.+)\.")
                   .HandleWith((match, entry) => currentTargetName = match.Groups[1].Value);

            // also added to other events, as this one is not reliable
        }

        void SetupMissCounts()
        {
            // miss count visible only for current character attacks

            // You miss with the longsword.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You miss with the .+\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName)
                           .MissesCount += 1);
        }

        void SetupShieldBlocks()
        {
            // Mature brown bear tries to maul you but you raise your shield and parry.
            // Aged black wolf tries to claw you but you raise your shield and parry.
            // [21:19:52] Belfesar tries to cut you but you raise your shield and parry.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) tries to (.+) you but you raise your shield and parry\.")
                   .HandleWith(
                       (match, entry) =>
                           UsingStatsFor(CharacterName, match.Groups[1].Value)
                               .GetActorByName(CharacterName)
                               .ShieldBlockCount += 1);

            // [21:20:50] You cut Belfesar but he raises his shield and parries.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You cut (.+) but he raises his shield and parries\.")
                   .HandleWith(
                       (match, entry) =>
                       {
                           UsingStatsFor(CharacterName, match.Groups[1].Value)
                               .GetActorByName(match.Groups[1].Value)
                               .ShieldBlockCount += 1;
                           currentTargetName = match.Groups[1].Value;
                       });

        }

        void SetupParryCounts()
        {
            // You safely parry with your longsword.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You (\w+) parry with .+\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).ParryCounts.IncrementForName(match.Groups[1].Value));

            // Aldur easily parries with a large maul.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) (\w+) parries with .+\.")
                   .HandleWith((match, entry) =>
                   {
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).ParryCounts.IncrementForName(match.Groups[2].Value);
                       currentTargetName = match.Groups[1].Value;
                   });
        }

        void SetupGlancingCounts()
        {
            // The attack to the right thigh glances off your armour.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"The attack to .+ glances off your armour\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(currentTargetName ?? string.Empty).GlancingBlowsCount += 1);

            // Your attack glances off Aged scorpion's armour.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Your attack glances off (.+)'s armour\.")
                   .HandleWith((match, entry) =>
                   {
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(CharacterName).GlancingBlowsCount += 1;
                       currentTargetName = match.Groups[1].Value;
                   });
        }

        void SetupEvadeCounts()
        {
            // You barely evade the blow to the head.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You (\w+) evade the blow to .+\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).EvadedCounts.IncrementForName(match.Groups[1].Value));

            // Belfesar barely evades the blow to the chest.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) (\w+) evades the blow to .+\.")
                   .HandleWith((match, entry) =>
                   {
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).EvadedCounts.IncrementForName(match.Groups[2].Value);
                       currentTargetName = match.Groups[1].Value;
                   });
        }

        void SetupTargetPreference()
        {
            // You move into position for the uppercenter parts of Old troll.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
               .Matches(@"You move into position for the (.+) parts of (.+)\.")
               .HandleWith((match, entry) =>
               {
                   UsingStatsFor(CharacterName, match.Groups[2].Value)
                       .GetActorByName(CharacterName)
                       .TargetPreferenceCounts.IncrementForName(match.Groups[1].Value.Trim());
                   currentTargetName = match.Groups[2].Value;
               });

            // Belfesar seems to target your left parts.
            // Belfesar targets your left parts.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) seems to target your (.+) parts\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).TargetPreferenceCounts.IncrementForName(match.Groups[2].Value.Trim()));
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) targets your (.+) parts\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).TargetPreferenceCounts.IncrementForName(match.Groups[2].Value.Trim()));

        }

        void SetupDamageCounts()
        {
            // Aged scorpion claws you hard in the stomach and irritates it.
            // You maul Aged hell hound deadly hard in the left paw and damage it.

            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) in the (.+) and (.+) it\.")
                   .HandleWith((match, entry) =>
                   {
                       var parseResult = damageEntryParser.Parse(match.Groups[1].Value);

                       if (parseResult.Valid)
                       {
                           var sourceActorName = Characterify(parseResult.SourceActorName);
                           var destActorName = Characterify(parseResult.DestActorName);
                           var type = parseResult.DamageType;
                           var strength = parseResult.DamageStrength;
                           var targetBodyPart = match.Groups[2].Value;
                           var damageCaused = match.Groups[3].Value;

                           var actor = UsingStatsFor(sourceActorName, destActorName)
                               .GetActorByName(sourceActorName);

                           if (string.Equals(sourceActorName, CharacterName, StringComparison.InvariantCultureIgnoreCase))
                           {
                               currentTargetName = destActorName;
                           }

                           var attack = new Attack()
                           {
                               Damage = damageCaused,
                               Strength = strength,
                               TargetBodyPart = targetBodyPart,
                               AttackType = type
                           };

                           actor.DamageCausedStats.Add(attack);
                       }
                       else
                       {
                           logger.Error("Skipping damage counts due to invalid parsing result, raw match: " + match.Value);
                       }
                   });
        }

        string Characterify(string actorName)
        {
            return string.Equals(actorName, "you", StringComparison.InvariantCultureIgnoreCase)
                ? CharacterName
                : actorName;
        }

        void SetupFocusLevels()
        {
            // levels:
            // You are not focused on combat.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You are not focused on combat\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.NotFocused);
            // You balance your feet and your soul.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You balance your feet and your soul\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Balanced);
            // You are now focused on the enemy and its every move.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You are now focused on the enemy and its every move\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Focused);
            // You feel lightning inside, quickening your reflexes.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You feel lightning inside, quickening your reflexes\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Lighting);
            // Your consciousness is lifted to a higher level, making you very attentive.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Your consciousness is lifted to a higher level, making you very attentive\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Lifted);
            // You feel supernatural. Invincible!
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You feel supernatural\. Invincible!")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Supernatural);
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You are already focused to the maximum\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Supernatural);

            // You lose some focus.
            // (reduces by one level)
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"You lose some focus\.")
                   .HandleWith((match, entry) =>
                       combatStatus.CurrentFocus.LowerByOneLevel());
        }

        void SetupIncomingAttackers()
        {
            // Aged large rat moves in to attack you.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"(.+) moves in to attack you\.")
                   .HandleWith((match, entry) =>
                       combatStatus.EnemyBeginsAttack(match.Groups[1].Value, entry));
        }

        void SetupSpellAttacks()
        {
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Your weapon freezes Aged black wolf\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).FreezingHits += 1);

            // Venom enters Old black wolf as you cut him.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Venom enters (.+) as you .+\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(CharacterName).AffectingHits += 1);

            // Flaming Aura
            // Your weapon burns Aged brown bear.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Your weapon burns (.+).")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(CharacterName).FlamingHits += 1);

            // Rotting Touch
            // Your cut affects Aged brown bear.
            matcher.WhenLogEntry().OfLogType(LogType.Combat)
                   .Matches(@"Your cut affects (.+)\.")
                   .HandleWith((match, entry) =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(CharacterName).AffectingHits += 1);
        }

        void SetupKillCounts()
        {
            // Aged hell hound is dead. R.I.P.
            matcher.WhenLogEntry().OfLogType(LogType.Event)
                   .Matches(@"(.+) is dead\. R\.I\.P\.")
                   .HandleWith((match, entry) =>
                   {
                       combatStatus.KillStatistics.IncrementForName(match.Groups[1].Value);
                       if (match.Groups[1].Value == currentTargetName)
                       {
                           UsingStatsFor(CharacterName, match.Groups[1].Value)
                               .GetActorByName(match.Groups[1].Value).SlainCount += 1;

                           if (lastSkillGain != null 
                               && lastSkillGain.Timestamp > entry.Timestamp - TimeSpan.FromSeconds(5))
                           {
                               UsingStatsFor(CharacterName, match.Groups[1].Value)
                                   .GetActorByName(CharacterName)
                                   .FightingSkillGained += lastSkillGain.Value;
                               lastSkillGain = null;
                           }
                           else
                           {
                               lastKill = new StampedValue<string>()
                               {
                                   Value = match.Groups[1].Value,
                                   Timestamp = entry.Timestamp
                               };
                           }

                           currentTargetName = null;
                       }
                   });
        }

        void SetupSkillGains()
        {
            matcher.WhenLogEntry().OfLogType(LogType.Skills)
                   .Matches(".+")
                   .HandleWith((match, entry) =>
                   {
                       var parsed = skillEntryParser.TryParseSkillInfoFromLogLine(entry);
                       if (parsed != null)
                       {
                           if (parsed.IsSkillName("fighting"))
                           {
                               if (lastKill != null &&
                                   lastKill.Timestamp > entry.Timestamp - TimeSpan.FromSeconds(5))
                               {
                                   UsingStatsFor(CharacterName, lastKill.Value)
                                       .GetActorByName(CharacterName)
                                       .FightingSkillGained += parsed.Gain ?? 0f;
                                   lastKill = null;
                               }
                               else
                               {
                                   lastSkillGain = new StampedValue<float>()
                                   {
                                       Value = parsed.Gain ?? 0f,
                                       Timestamp = entry.Timestamp
                                   };
                               }
                           }
                       }
                   });
        }

        void OnlyWhenCurrentTargetKnown(Action action)
        {
            if (!string.IsNullOrWhiteSpace(currentTargetName))
            {
                action();
            }
        }

        Data.Combat.CombatActorPairStats UsingStatsFor(string actorOne, string actorTwo)
        {
            return combatStatus.GetStatsFor(actorOne, actorTwo);
        }

        public bool ProcessEntry(LogEntry logEntry, LogType logType)
        {
            return matcher.Match(logEntry, logType);
        }

        public async Task<bool> ProcessEntryAsync(LogEntry logEntry, LogType logType)
        {
            return await matcher.MatchAsync(logEntry, logType);
        }

        public IEnumerable<LogType> GetRequiredLogs()
        {
            return matcher.GetRequiredLogs();
        } 

        class EntryMatchingRuleEngine
        {
            readonly List<MatchRule> rules = new List<MatchRule>();
            Dictionary<LogType, MatchRule[]> rulesLookupCache = new Dictionary<LogType, MatchRule[]>();
            bool cacheRebuildRequired = false;

            public IRuleBuilder WhenLogEntry()
            {
                var rule = new MatchRule();
                rules.Add(rule);
                cacheRebuildRequired = true;
                return rule;
            }

            public bool Match(LogEntry entry, LogType logType)
            {
                RebuildIfRequired();

                MatchRule[] matchers;
                if (rulesLookupCache.TryGetValue(logType, out matchers))
                {
                    foreach (var matchRule in matchers)
                    {
                        var match = Regex.Match(entry.Content, matchRule.Pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            matchRule.MatchHandler(match, entry);
                            return true;
                        }
                    }
                }

                return false;
            }

            public async Task<bool> MatchAsync(LogEntry entry, LogType logType)
            {
                RebuildIfRequired();

                MatchRule[] matchers;
                var result = false;
                if (rulesLookupCache.TryGetValue(logType, out matchers))
                {
                    await Task.Run(() =>
                    {
                        foreach (var matchRule in matchers)
                        {
                            var match = Regex.Match(entry.Content, matchRule.Pattern, RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                matchRule.MatchHandler(match, entry);
                                result = true;
                                break;
                            }
                        }
                    });
                }

                return result;
            }

            public IEnumerable<LogType> GetRequiredLogs()
            {
                RebuildIfRequired();
                return rulesLookupCache.Keys;
            } 

            void RebuildIfRequired()
            {
                if (cacheRebuildRequired)
                {
                    BuildRuleCache();
                    ValidateRules();
                    cacheRebuildRequired = false;
                }
            }

            void BuildRuleCache()
            {
                rulesLookupCache = rules.GroupBy(rule => rule.LogType)
                                        .ToDictionary(rule => rule.Key, grouping => grouping.ToArray());
            }

            void ValidateRules()
            {
                foreach (var matchRule in rules)
                {
                    ThrowIfInvalid(matchRule);
                }
            }

            void ThrowIfInvalid(MatchRule rule)
            {
                if (rule.MatchHandler == null) throw new InvalidOperationException("rule has no handler");
                if (string.IsNullOrWhiteSpace(rule.Pattern)) throw new InvalidOperationException("rule has empty pattern");
                if (rule.LogType == LogType.Unspecified) throw new InvalidOperationException("rule has unspecified log type");
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                // validating if pattern compiles
                Regex.Match("sample", rule.Pattern);
            }
        }

        class MatchRule : IRuleBuilder
        {
            public string Pattern { get; private set; }
            public Action<Match, LogEntry> MatchHandler { get; private set; }
            public LogType LogType { get; private set; }

            IRuleBuilder IRuleBuilder.Matches([RegexPattern] string pattern)
            {
                Pattern = pattern;
                return this;
            }

            IRuleBuilder IRuleBuilder.HandleWith(Action<Match, LogEntry> handler)
            {
                MatchHandler = handler;
                return this;
            }

            IRuleBuilder IRuleBuilder.OfLogType(LogType logType)
            {
                LogType = logType;
                return this;
            }
        }

        interface IRuleBuilder
        {
            IRuleBuilder Matches([RegexPattern] string pattern);
            IRuleBuilder HandleWith(Action<Match, LogEntry> handler);
            IRuleBuilder OfLogType(LogType logType);
        }

        class DamageEntryParser
        {
            readonly ILogger logger;

            public DamageEntryParser(ILogger logger)
            {
                if (logger == null)
                    throw new ArgumentNullException("logger");
                this.logger = logger;

                pluralizedAttackTypes = attackTypes.Select(s => s + "s").ToArray();
            }

            // order is important (composites first to check!)
            readonly string[] attackStrengths = new[]
            {
                "Very Lightly",
                "Pretty Hard",
                "Very Hard",
                "Extremely Hard",
                "Deadly Hard",
                "Lightly",
                "Hard",
            };

            readonly DefinedAttackType[] attackTypes = new[]
            {
                new DefinedAttackType("maul"),
                new DefinedAttackType("cut"),
                new DefinedAttackType("pierce"),
                new DefinedAttackType("hit"),
                new DefinedAttackType("claw"),
                new DefinedAttackType("bite"),
                new DefinedAttackType("burn"),
                new DefinedAttackType("kick"),
                new DefinedAttackType("pound"),
                new DefinedAttackType("squeeze"),
                new DefinedAttackType("tailwhip"),
                new DefinedAttackType("wingbuff"),
                new DefinedAttackType("hurt"),
            };

            readonly string[] pluralizedAttackTypes;

            readonly char[] delimiters = new[] { ' ' };

            public DamageEntryParseResult Parse(string logEntryContentPart)
            {
                var result = new DamageEntryParseResult();

                var parts = logEntryContentPart.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var delimitationResult = TryEstablishAttackTypeDelimiter(parts);

                LogIfDelimiterNotEstablished(delimitationResult, logEntryContentPart);

                if (delimitationResult != null)
                {
                    var attackerParts = parts.Take(delimitationResult.BeginIndex);
                    var remainingPart = parts.Skip(delimitationResult.EndIndex + 1);

                    result.SourceActorName = string.Join(" ", attackerParts);
                    result.DamageType = delimitationResult.AttackType;

                    var combinedRemainingPart = string.Join(" ", remainingPart);
                    var attackStrength = TryParseAttackStrength(combinedRemainingPart);

                    LogIfAttackStrengthUnknown(attackStrength, combinedRemainingPart);

                    if (attackStrength != null)
                    {
                        result.DamageStrength = attackStrength;
                        result.DestActorName =
                            Regex.Replace(combinedRemainingPart, attackStrength, "", RegexOptions.IgnoreCase).Trim();
                       
                        result.Valid = true;
                    }
                }

                return result;
            }

            void LogIfAttackStrengthUnknown(string attackStrength, string combinedRemainingPart)
            {
                if (attackStrength == null)
                {
                    logger.Error("DamageEntryParser: Could not parse attack strength from log entry, source string: " + combinedRemainingPart);
                }
            }

            void LogIfDelimiterNotEstablished(DelimitationResult result, string logEntryContentPart)
            {
                if (result == null)
                {
                    logger.Error("DamageEntryParser: Could not split the log entry part, source string: " + logEntryContentPart);
                }
                else if (result.BeginIndex == 0)
                {
                    logger.Error("DamageEntryParser: Log entry part was split but left side is empty, source string: " + logEntryContentPart);
                }
            }

            string TryParseAttackStrength(string combinedRemainingPart)
            {
                return
                    attackStrengths.FirstOrDefault(
                        s => combinedRemainingPart.Contains(s, StringComparison.InvariantCultureIgnoreCase));
            }

            DelimitationResult TryEstablishAttackTypeDelimiter(string[] parts)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    var specialAttackResult = TryParseSpecialAttack(parts, i);
                    if (specialAttackResult != null)
                    {
                        return specialAttackResult;
                    }
                    else
                    {
                        var part = parts[i];
                        foreach (var attackType in attackTypes)
                        {
                            if (attackType.Matches(part))
                            {
                                return new DelimitationResult().SetSingleIndex(i).SetAttackType(attackType.Singular);
                            }
                        }
                    }
                }
                return null;
            }

            DelimitationResult TryParseSpecialAttack(string[] parts, int i)
            {
                //todo
                //"hits the groin of", // Low rider
                if (parts.Length > i + 3 
                    && parts[i] == "hits" 
                    && parts[i + 1] == "the" 
                    && parts[i + 2] == "groin"
                    && parts[i + 3] == "of")
                {
                    return new DelimitationResult().SetAttackType("Low rider").SetIndexSpan(i, i + 3);
                }

                //"lunge and cut", // Bloodscion
                if (parts.Length > i + 2
                    && parts[i] == "lunge"
                    && parts[i + 1] == "and"
                    && parts[i + 2] == "cut")
                {
                    return new DelimitationResult().SetAttackType("Bloodscion").SetIndexSpan(i, i + 2);
                }

                //"engrave", // Back breaker
                if (parts[i] == "engrave")
                {
                    return new DelimitationResult().SetAttackType("Back breaker").SetSingleIndex(i);
                }

                return null;
            }

            class DefinedAttackType
            {
                public DefinedAttackType(string singular, bool autopluralize = true)
                {
                    Singular = singular;
                    if (autopluralize) Plural = singular + "s";
                }

                public string Singular { get; set; }
                public string Plural { get; set; }

                public bool Matches(string source)
                {
                    if (string.Equals(source, Singular, StringComparison.InvariantCultureIgnoreCase)
                        || (Plural != null && string.Equals(source, Plural, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return true;
                    }

                    return false;
                }
            }

            class DelimitationResult
            {
                public string AttackType { get; set; }
                public int BeginIndex { get; set; }
                public int EndIndex { get; set; }

                public DelimitationResult SetSingleIndex(int index)
                {
                    BeginIndex = EndIndex = index;
                    return this;
                }

                public DelimitationResult SetIndexSpan(int start, int end)
                {
                    BeginIndex = start;
                    EndIndex = end;
                    return this;
                }

                public DelimitationResult SetAttackType(string attackType)
                {
                    AttackType = attackType;
                    return this;
                }
            }
        }

        class DamageEntryParseResult
        {
            public bool Valid { get; set; }

            public string SourceActorName { get; set; }
            public string DestActorName { get; set; }
            public string DamageType { get; set; }
            public string DamageStrength { get; set; }
        }

        class StampedValue<T>
        {
            public DateTime Timestamp { get; set; }
            public T Value { get; set; }
        }
    }
}
