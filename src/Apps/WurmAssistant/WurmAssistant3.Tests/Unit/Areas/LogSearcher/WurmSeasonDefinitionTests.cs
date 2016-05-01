using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Contracts;
using NUnit.Framework;

namespace AldursLab.WurmAssistant3.Tests.Unit.Areas.LogSearcher
{
    class WurmSeasonDefinitionTests : AssertionHelper
    {
        [Test]
        public void WhenSameYear_LengthCorrect()
        {
            var def = new WurmSeasonDefinition()
            {
                DayBegin = 5,
                DayEnd = 15
            };

            Expect(def.Length, EqualTo(10));
        }

        [Test]
        public void WhenCrossYears_LengthCorrect()
        {
            var def = new WurmSeasonDefinition()
            {
                DayBegin = 250,
                DayEnd = 50
            };

            Expect(def.Length, EqualTo(WurmCalendar.DaysInYear - 250 + 50));
        }
    }
}
