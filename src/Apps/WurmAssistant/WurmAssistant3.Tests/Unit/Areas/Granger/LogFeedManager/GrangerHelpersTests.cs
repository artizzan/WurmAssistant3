using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [TestCase("She will deliver in about 4 days.", 5, 0, 0)]
        [TestCase("She will deliver in about 1 day.", 2, 0, 0)]
        [TestCase("You feel confident she will give birth in 6 days.", 6, 0, 0)]
        [TestCase("You feel confident she will give birth in 1 day.", 1, 0, 0)]
        [TestCase("You feel confident she will give birth in 6 days, 5 hours, 35 minutes.", 6, 5, 35)]
        [TestCase("You feel confident he will give birth in 6 days, 5 hours, 35 minutes.", 6, 5, 35)]
        [TestCase("You feel confident she will give birth in 6 days, 5 hours.", 6, 5, 0)]
        [TestCase("You feel confident she will give birth in 1 day, 1 hour.", 1, 1, 0)]
        [TestCase("You feel confident she will give birth in 6 days, 1 hour.", 6, 1, 0)]
        [TestCase("You feel confident he will give birth in 6 days, 1 hour.", 6, 1, 0)]
        [TestCase("You feel confident she will give birth in 6 hours.", 0, 6, 0)]
        [TestCase("You feel confident she will give birth in 1 hour.", 0, 1, 0)]
        [TestCase("You feel confident she will give birth in 9 minutes.", 0, 0, 9)]
        [TestCase("You feel confident he will give birth in 9 minutes.", 0, 0, 9)]
        [TestCase("You predict she will give birth in 6 days, 19 hours.", 6, 19, 0)]
        [TestCase("You predict she will give birth in 1 days, 19 hours.", 1, 19, 0)]
        [TestCase("You predict she will give birth in 19 hours.", 0, 19, 0)]
        [TestCase("You predict she will give birth in 1 hour.", 0, 1, 0)]
        [TestCase("You predict she will give birth in 6 days.", 6, 0, 0)]
        [TestCase("You predict she will give birth in 1 day.", 1, 0, 0)]
        [TestCase("You are absolutely certain she will give birth in 3 days, 5 hours, 2 minutes.", 3, 5, 2)]
        [TestCase("You are absolutely certain he will give birth in 3 days, 5 hours, 2 minutes.", 3, 5, 2)]
        [TestCase("You are absolutely certain she will give birth in 1 day, 1 hour, 1 minute.", 1, 1, 1)]
        [TestCase("You are absolutely certain he will give birth in 1 day, 1 hour, 1 minute.", 1, 1, 1)]
        public void ParsePregnantUntil(string line, int expectedDays, int expectedHours, int expectedMinutes)
        {
            var now = new DateTime(2021, 1, 1, 0, 0, 0);
            var result = GrangerHelpers.TryParsePregnantUntil(line, new GrangerDebugLogger(new LoggerStub()), now);
            Assert.IsNotNull(result);
            Assert.AreEqual(now + TimeSpan.FromDays(expectedDays) + TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes), result.Value);
        }
    }
}
