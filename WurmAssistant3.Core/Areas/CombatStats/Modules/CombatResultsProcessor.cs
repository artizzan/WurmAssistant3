using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    class CombatResultsProcessor
    {
        readonly CombatStatus combatStatus;
        readonly EntryMatcher matcher = new EntryMatcher();

        string currentTargetName = string.Empty;
        string CharacterName { get { return combatStatus.CharacterName; } }

        public CombatResultsProcessor(CombatStatus combatStatus)
        {
            if (combatStatus == null) throw new ArgumentNullException("combatStatus");
            this.combatStatus = combatStatus;

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
        }

        void SetupCurrentTargetRecognition()
        {
            // does not log for other characters.

            // You try to maul a Aged troll.
            matcher.WhenLogEntry()
                   .Matches(@"You try to \w+ a (.+)\.")
                   .HandleWith(match => currentTargetName = match.Groups[1].Value);

        }

        void SetupMissCounts()
        {
            // miss count visible only for current character attacks

            // You miss with the longsword.
            matcher.WhenLogEntry()
                   .Matches(@"You miss with the .+\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName)
                           .MissesCount += 1);
        }

        void SetupShieldBlocks()
        {
            // Mature brown bear tries to maul you but you raise your shield and parry.
            // Aged black wolf tries to claw you but you raise your shield and parry.
            // [21:19:52] Belfesar tries to cut you but you raise your shield and parry.
            matcher.WhenLogEntry()
                   .Matches(@"(.+) tries to (.+) you but you raise your shield and parry\.")
                   .HandleWith(
                       match =>
                           UsingStatsFor(CharacterName, match.Groups[1].Value)
                               .GetActorByName(CharacterName)
                               .ShieldBlockCount += 1);

            // [21:20:50] You cut Belfesar but he raises his shield and parries.
            matcher.WhenLogEntry()
                   .Matches(@"You cut (.+) but he raises his shield and parries\.")
                   .HandleWith(
                       match =>
                           UsingStatsFor(CharacterName, match.Groups[1].Value)
                               .GetActorByName(match.Groups[1].Value)
                               .ShieldBlockCount += 1);

        }

        void SetupParryCounts()
        {
            // You safely parry with your longsword.
            matcher.WhenLogEntry()
                   .Matches(@"You safely parry with .+\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).ParryCounts.IncrementForName(match.Groups[2].Value));

            // Aldur easily parries with a large maul.
            matcher.WhenLogEntry()
                   .Matches(@"(.+) easily parries with .+\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).ParryCounts.IncrementForName(match.Groups[2].Value));
        }

        void SetupGlancingCounts()
        {
            // The attack to the right thigh glances off your armour.
            matcher.WhenLogEntry()
                   .Matches(@"The attack to .+ glances off your armour\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).GlancingReceivedCount += 1);

            // Your attack glances off Aged scorpion's armour.
            matcher.WhenLogEntry()
                   .Matches(@"Your attack glances off (.+)'s armour\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).GlancingReceivedCount += 1);
        }

        void SetupEvadeCounts()
        {
            // You barely evade the blow to the head.
            matcher.WhenLogEntry()
                   .Matches(@"You (\w+) evade the blow to .+\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, currentTargetName ?? string.Empty)
                           .GetActorByName(CharacterName).EvadedCounts.IncrementForName(match.Groups[1].Value));

            // Belfesar barely evades the blow to the chest.
            matcher.WhenLogEntry()
                   .Matches(@"(.+) (\w+) evades the blow to .+\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).EvadedCounts.IncrementForName(match.Groups[2].Value));
        }

        void SetupTargetPreference()
        {
            // You try to move into position for the uppercenter parts of Aged black wolf.
            matcher.WhenLogEntry()
               .Matches(@"You try to move into position for the (.+) parts of (.+)\.")
               .HandleWith(match =>
                   UsingStatsFor(CharacterName, match.Groups[2].Value)
                       .GetActorByName(CharacterName).TargetPreferenceCounts.IncrementForName(match.Groups[1].Value.Trim()));

            // Belfesar seems to target your left parts.
            matcher.WhenLogEntry()
                   .Matches(@"(.+) seems to target your (.+) parts\.")
                   .HandleWith(match =>
                       UsingStatsFor(CharacterName, match.Groups[1].Value)
                           .GetActorByName(match.Groups[1].Value).TargetPreferenceCounts.IncrementForName(match.Groups[2].Value.Trim()));

        }

        void SetupDamageCounts()
        {
            // todo universal parser

            // ! need: list of all possible damage type strings

            // take 4 first groups
            // split groups into words and put in array
            // go over array until "damage type" matches, note the index
            // take part left of index as damage causer
            // take index part as damage type
            // merge part right of index into one string
            // try to match attack strength for this string
            // if matched,
            //  use it as damage strength
            //  replace string to eliminate this damage strength, trim, use as damage receiver

            // Aged scorpion claws you hard in the stomach and irritates it.
            // groups: enemy name, type of attack, strength of attack, target of attack, damage of attack
            matcher.WhenLogEntry()
                   .Matches(@"(.+) (\w+) you (.+) in the (.+) and (.+) it\.")
                   .HandleWith(match =>
                   {
                       var enemyName = match.Groups[1].Value;
                       var type = match.Groups[2].Value;
                       var strength = match.Groups[3].Value;
                       var targetBodyPart = match.Groups[4].Value;
                       var damageCaused = match.Groups[5].Value;

                       var actor = UsingStatsFor(CharacterName, enemyName)
                           .GetActorByName(enemyName);

                       var attack = new Attack()
                       {
                           Damage = damageCaused,
                           Strength = strength,
                           TargetBodyPart = targetBodyPart,
                           Type = type
                       };

                       actor.DamageCausedStats.Add(attack);
                   });

            // You maul Aged hell hound deadly hard in the left paw and damage it.
            // groups: type of attack, [complex: target name and strength of attack], target of attack, damage of attack
            matcher.WhenLogEntry()
                   .Matches(@"You (\w+) (.+) in the (.+) and (.+) it\.")
                   .HandleWith(match =>
                   {
                       var enemyName = GetTargetNameFromDmgCausedLine(match.Groups[2].Value);
                       var type = match.Groups[1].Value;
                       var strength = GetStrOfAttackFromDmgCausedLine(match.Groups[2].Value);
                       var targetBodyPart = match.Groups[3].Value;
                       var damageCaused = match.Groups[4].Value;

                       var actor = UsingStatsFor(CharacterName, enemyName)
                           .GetActorByName(CharacterName);

                       var attack = new Attack()
                       {
                           Damage = damageCaused,
                           Strength = strength,
                           TargetBodyPart = targetBodyPart,
                           Type = type
                       };

                       actor.DamageCausedStats.Add(attack);
                   });
        }

        readonly string[] attackStrengths = new[]
        {"Very Lightly", "Lightly", "Pretty Hard", "Hard", "Very Hard", "Extremely Hard", "Deadly Hard"};

        string GetStrOfAttackFromDmgCausedLine(string linePart)
        {
            return attackStrengths.Where(linePart.Contains).FirstOrValue(() => "Unknown");
        }

        string GetTargetNameFromDmgCausedLine(string linePart)
        {
            foreach (var attackStrength in attackStrengths)
            {
                linePart = linePart.Replace(attackStrength + " ", "");
            }
            return linePart.Trim();
        }

        void SetupFocusLevels()
        {
            // levels:
            // You are not focused on combat.
            matcher.WhenLogEntry()
                   .Matches(@"You are not focused on combat\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.NotFocused);
            // You balance your feet and your soul.
            matcher.WhenLogEntry()
                   .Matches(@"You balance your feet and your soul\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Balanced);
            // You are now focused on the enemy and its every move.
            matcher.WhenLogEntry()
                   .Matches(@"You are now focused on the enemy and its every move\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Focused);
            // You feel lightning inside, quickening your reflexes.
            matcher.WhenLogEntry()
                   .Matches(@"You feel lightning inside, quickening your reflexes\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Lighting);
            // Your consciousness is lifted to a higher level, making you very attentive.
            matcher.WhenLogEntry()
                   .Matches(@"Your consciousness is lifted to a higher level, making you very attentive\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Lifted);
            // You feel supernatural. Invincible!
            matcher.WhenLogEntry()
                   .Matches(@"You feel supernatural. Invincible\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.FocusLevel = FocusLevel.Supernatural);

            // You lose some focus.
            // (reduces by one level)
            matcher.WhenLogEntry()
                   .Matches(@"You lose some focus\.")
                   .HandleWith(match =>
                       combatStatus.CurrentFocus.LowerByOneLevel());
        }

        void SetupIncomingAttackers()
        {
            // Aged large rat moves in to attack you.
            matcher.WhenLogEntry()
                   .Matches(@"(.+) moves in to attack you\.")
                   .HandleWith(match =>
                       combatStatus.EnemyBeginsAttack(match.Groups[1].Value));
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

        public void ProcessEntry(LogEntry logEntry)
        {
            matcher.Match(logEntry);
        }

        class EntryMatcher
        {
            readonly List<MatchRule> rules = new List<MatchRule>();

            public IRuleBuilder WhenLogEntry()
            {
                var rule = new MatchRule();
                rules.Add(rule);
                return rule;
            }

            public void ValidateRules()
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
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                // validating if pattern compiles
                Regex.IsMatch("sample", rule.Pattern);
            }

            public void Match(LogEntry entry)
            {
                foreach (var matchRule in rules)
                {
                    var match = Regex.Match(entry.Content, matchRule.Pattern);
                    if (match.Success)
                    {
                        matchRule.MatchHandler(match);
                        return;
                    }
                }
            }
        }

        class MatchRule : IRuleBuilder
        {
            public string Pattern { get; private set; }
            public Action<Match> MatchHandler { get; private set; }

            IRuleBuilder IRuleBuilder.Matches([RegexPattern] string pattern)
            {
                Pattern = pattern;
                return this;
            }

            IRuleBuilder IRuleBuilder.HandleWith(Action<Match> handler)
            {
                MatchHandler = handler;
                return this;
            }
        }

        interface IRuleBuilder
        {
            IRuleBuilder Matches([RegexPattern] string pattern);
            IRuleBuilder HandleWith(Action<Match> handler);
        }
    }
}
