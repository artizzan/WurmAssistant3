using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    class CombatResultsProcessor
    {
        readonly CombatResults combatResults;
        readonly string characterName;
        string currentTargetName = string.Empty;

        readonly EntryMatcher matcher = new EntryMatcher();

        public CombatResultsProcessor(CombatResults combatResults, string characterName)
        {
            if (combatResults == null) throw new ArgumentNullException("combatResults");
            if (characterName == null) throw new ArgumentNullException("characterName");
            this.combatResults = combatResults;
            this.characterName = characterName;

            SetupRules();
        }

        void SetupRules()
        {
            // You try to maul a Aged troll.
            // does not log for other characters.
            matcher.WhenLogEntry()
                   .Matches(@"You try to \w+ a (.+)\.")
                   .HandleWith(match => currentTargetName = match.Groups[1].Value);

            // Mature brown bear tries to maul you but you raise your shield and parry.
            // Aged black wolf tries to claw you but you raise your shield and parry.
            // todo: logs for other players??
            matcher.WhenLogEntry()
                   .Matches(@"(.+) tries to (.+) you but you raise your shield and parry\.")
                   .HandleWith(
                       match =>
                           GetStatsFor(characterName, match.Groups[1].Value)
                               .GetActorByName(characterName)
                               .ShieldBlockCount += 1);

            // You miss with the longsword.
            // miss count visible only for current character attacks
            matcher.WhenLogEntry()
                   .Matches(@"You miss with the .+\.")
                   .HandleWith(
                       match => OnlyWhenCurrentTargetKnown(() =>
                           GetStatsFor(characterName, currentTargetName)
                               .GetActorByName(characterName)
                               .MissesCount += 1));

            // Aged huge spider skillfully evades the blow to the stomach.

            // You cut Aged huge spider extremely hard in the stomach and damage it.


            // evasion levels: skillfully, barely
        }

        void OnlyWhenCurrentTargetKnown(Action action)
        {
            if (!string.IsNullOrWhiteSpace(currentTargetName))
            {
                action();
            }
        }

        Data.Combat.CombatStats GetStatsFor(string actorOne, string actorTwo)
        {
            return combatResults.GetStatsFor(actorOne, actorTwo);
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
