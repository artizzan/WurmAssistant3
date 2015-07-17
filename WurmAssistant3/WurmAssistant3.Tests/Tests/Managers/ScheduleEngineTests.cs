using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core;
using AldurSoft.Core.Testing;
using AldurSoft.WurmAssistant3.Engine.Systems;
using AldurSoft.WurmAssistant3.Systems;

using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmAssistant3.Tests.Tests.Managers
{
    [TestFixture]
    public class ScheduleEngineTests : FixtureBase
    {
        private ScheduleEngine system;
        private ITimer timer;

        [SetUp]
        public void Setup()
        {
            timer = Automocker.Create<ITimer>();
            var timerFactory = Automocker.Create<ITimerFactory>();
            timerFactory.GetMock().Setup(factory => factory.Create()).Returns(() => timer);
            system = new ScheduleEngine(timerFactory);
        }

        [Test]
        public void TimerInvokes()
        {
            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNow = DateTime.Now;
                bool invoked = false;
                system.RegisterForUpdates(TimeSpan.FromSeconds(1), info => invoked = true);
                RaiseTick();
                Expect(invoked, True);

                invoked = false;
                RaiseTick();
                Expect(invoked, False);

                scope.LocalNow = Time.Clock.LocalNow + TimeSpan.FromMilliseconds(990);
                RaiseTick();
                Expect(invoked, False);

                scope.LocalNow = Time.Clock.LocalNow + TimeSpan.FromMilliseconds(20);
                RaiseTick();
                Expect(invoked, True);

                timer.GetMock().Verify(t => t.Start(), Times.Once());
            }
        }

        private void RaiseTick()
        {
            timer.GetMock().Raise(t => t.Tick += null, new EventArgs());
        }
    }
}
