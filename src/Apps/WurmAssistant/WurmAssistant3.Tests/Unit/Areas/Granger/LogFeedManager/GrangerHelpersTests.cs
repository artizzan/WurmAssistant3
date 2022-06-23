using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager;
using AldursLab.WurmAssistant3.Areas.Logging;
using NUnit.Framework;

namespace AldursLab.WurmAssistant3.Tests.Unit.Areas.Granger.LogFeedManager
{
    class GrangerHelpersTests
    {
        [TestCase("His mother is the old fat Painthop. His father is the venerable fat Starkclip.", "Painthop", "Starkclip")]
        [TestCase("His Mother is the old fat Painthop. His Father is the venerable fat Starkclip.", "Painthop", "Starkclip")]
        [TestCase("His mother was Painthop. His father was Starkclip.", "Painthop", "Starkclip")]
        [TestCase("Her mother is the old fat Painthop. Her father is the venerable fat Starkclip.", "Painthop", "Starkclip")]
        [TestCase("Her Mother is the old fat Painthop. Her Father is the venerable fat Starkclip.", "Painthop", "Starkclip")]
        [TestCase("Her mother was Painthop. Her father was Starkclip.", "Painthop", "Starkclip")]
        [TestCase("Her mother is the aged fat Soulwar. Her father is the aged fat Greysouth.", "Soulwar", "Greysouth")]
        public void ParseCreatureName(string parentLine, string expectedMother, string expectedFather)
        {
            var result = GrangerHelpers.ExtractParentNames(parentLine, new GrangerDebugLogger(new LoggerStub()));
            Assert.AreEqual(expectedMother, result.Mother);
            Assert.AreEqual(expectedFather, result.Father);
        }

        [TestCaseSource(nameof(GenerateCasesForParsePregnantUntil))]
        public void ParsePregnantUntil(string line, int expectedDays, int expectedHours, int expectedMinutes)
        {
            var now = new DateTime(2021, 1, 1, 0, 0, 0);
            var result = GrangerHelpers.TryParsePregnantUntil(line, new GrangerDebugLogger(new LoggerStub()), now);
            Assert.IsNotNull(result);
            Assert.AreEqual(now + TimeSpan.FromDays(expectedDays) + TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes), result.Value);
        }

        static IEnumerable<object[]> GenerateCasesForParsePregnantUntil()
        {
            // legacy pregnancy messages, still needed for old wurm unlimited game versions
            // note: final predicted date is more accurate when +1 day is added
            // todo: unknown message for "deliver any moment" or smtg like that

            yield return new object[] { "She will deliver in about 1 day.", 2, 0, 0 };
            yield return new object[] { "She will deliver in about 4 days.", 5, 0, 0 };

            // new eloquent pregnancy messages, lets hope these are the all possible combinations!

            string[] part1Strings = new[]
            {
                "You feel confident {gender} will give birth",
                "You predict {gender} will give birth",
                "You are absolutely certain {gender} will give birth",
                "You feel very confident {gender} will give birth",
                "You guess that {gender} might give birth"
            };

            Tuple<string, int, int, int>[] part2Strings = new[]
            {
                new Tuple<string, int, int, int>("in 1 day.", 1, 0, 0),
                new Tuple<string, int, int, int>("in 4 days.", 4, 0, 0),
                new Tuple<string, int, int, int>("in 1 hour.", 0, 1, 0),
                new Tuple<string, int, int, int>("in 16 hours.", 0, 16, 0),
                new Tuple<string, int, int, int>("in 1 minute.", 0, 0, 1),
                new Tuple<string, int, int, int>("in 37 minutes.", 0, 0, 37),
                new Tuple<string, int, int, int>("in 1 day, 1 hour.", 1, 1, 0),
                new Tuple<string, int, int, int>("in 1 day, 1 hour, 1 minute.", 1, 1, 1),
                new Tuple<string, int, int, int>("in 1 day, 1 minute.", 1, 0, 1),
                new Tuple<string, int, int, int>("in 1 hour, 1 minute.", 0, 1, 1),
                new Tuple<string, int, int, int>("in 1 minute.", 0, 0, 1),
                new Tuple<string, int, int, int>("in 13 days, 12 hours.", 13, 12, 0),
                new Tuple<string, int, int, int>("in 13 days, 12 hours, 37 minutes.", 13, 12, 37),
                new Tuple<string, int, int, int>("in 13 days, 37 minutes.", 13, 0, 37),
                new Tuple<string, int, int, int>("in 12 hours, 37 minutes.", 0, 12, 37),
                new Tuple<string, int, int, int>("in 37 minutes.", 0, 0, 37),
            };

            // note: it may feel silly to include "he" gender, but... it's Wurm, lets assume the unassumable
            string[] genders = new string[] {"he", "she", "it"};

            foreach (var p1 in part1Strings)
            {
                foreach (var p2 in part2Strings)
                {
                    foreach (var gender in genders)
                    {
                        var testCase = new object[]
                        {
                            $"{p1.Replace("{gender}", gender).Capitalize()} {p2.Item1}",
                            p2.Item2,
                            p2.Item3,
                            p2.Item4
                        };

                        yield return testCase;
                    }
                }
            }
        }
    }
}
