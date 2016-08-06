using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Utils.Collections;
using JetBrains.Annotations;
using Notify;
using NUnit.Framework;

namespace AldursLab.WurmAssistant3.Tests.Unit.Utils.Collections
{
    public class MemberChangeNotifyingListTests : AssertionHelper
    {
        [Test]
        public void NotifiesOnListChange()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List = new MemberChangeNotifyingList<Bar>();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListAdd()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Add(new Bar());

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListInsert()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;
            
            fixture.Foo.List.Insert(0, new Bar());

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListRemove()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.List.Add(bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Remove(bar);

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListClear()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Clear();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListIndexerAdd()
        {
            var fixture = CreateFixture();
            fixture.Foo.List.Add(new Bar());

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List[0] = new Bar();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListIndexerRemove()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.List.Add(bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List[0] = null;

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnListMemberChange()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.List.Add(bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.First().Value = "changed";

            Expect(notified, True);

        }

        Fixture CreateFixture()
        {
            return new Fixture();
        }

        class Fixture
        {
            public Foo Foo { get; } = new Foo();
            public Tracker Tracker { get; } = new Tracker();

            public Fixture()
            {
                Tracker.Track(Foo);
            }
        }

        [DataContract]
        class Foo : INotifyPropertyChanged
        {
            MemberChangeNotifyingList<Bar> list = new MemberChangeNotifyingList<Bar>();

            [DataMember]
            public IList<Bar> List
            {
                get { return list; }
                set
                {
                    if (Equals(value, list)) return;
                    list = (MemberChangeNotifyingList<Bar>)value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataContract]
        class Bar : INotifyPropertyChanged
        {
            string value;

            [DataMember]
            public string Value
            {
                get { return value; }
                set
                {
                    if (value == this.value) return;
                    this.value = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MemberChangeNotifyingHashsetTests : AssertionHelper
    {
        [Test]
        public void NotifiesOnHashsetChange()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List = new MemberChangeNotifyingHashset<Bar>();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnHashsetAdd()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Add(new Bar());

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnHashsetRemove()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.List.Add(bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Remove(bar);

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnHashsetClear()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.Clear();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnHashsetMemberChange()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.List.Add(bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.List.First().Value = "changed";

            Expect(notified, True);

        }

        Fixture CreateFixture()
        {
            return new Fixture();
        }

        class Fixture
        {
            public Foo Foo { get; } = new Foo();
            public Tracker Tracker { get; } = new Tracker();

            public Fixture()
            {
                Tracker.Track(Foo);
            }
        }

        [DataContract]
        class Foo : INotifyPropertyChanged
        {
            MemberChangeNotifyingHashset<Bar> list = new MemberChangeNotifyingHashset<Bar>();

            [DataMember]
            public ISet<Bar> List
            {
                get { return list; }
                set
                {
                    if (Equals(value, list)) return;
                    list = (MemberChangeNotifyingHashset<Bar>)value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataContract]
        class Bar : INotifyPropertyChanged
        {
            string value;

            [DataMember]
            public string Value
            {
                get { return value; }
                set
                {
                    if (value == this.value) return;
                    this.value = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
