using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AldursLab.WurmAssistant3.Utils.Collections;
using JetBrains.Annotations;
using Notify;
using NUnit.Framework;

namespace AldursLab.WurmAssistant3.Tests.Unit.Utils.Collections
{
    public class MemberChangeNotifyingDictionaryTests : AssertionHelper
    {
        [Test]
        public void NotifiesOnDictionaryChange()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary = new MemberChangeNotifyingDictionary<string, Bar>();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryAdd()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary.Add("key", new Bar());

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryRemove()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.Dictionary.Add("key", bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary.Remove("key");

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryClear()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary.Clear();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryIndexerAdd()
        {
            var fixture = CreateFixture();

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary["key"] = new Bar();

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryIndexerRemove()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.Dictionary.Add("key", bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary["key"] = null;

            Expect(notified, True);
        }

        [Test]
        public void NotifiesOnDictionaryMemberChange()
        {
            var fixture = CreateFixture();
            var bar = new Bar();
            fixture.Foo.Dictionary.Add("key", bar);

            bool notified = false;
            fixture.Tracker.Changed += tracker => notified = true;

            fixture.Foo.Dictionary["key"].Value = "changed";

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
            MemberChangeNotifyingDictionary<string, Bar> dictionary = new MemberChangeNotifyingDictionary<string, Bar>();

            [DataMember]
            public IDictionary<string, Bar> Dictionary
            {
                get { return dictionary; }
                set
                {
                    if (Equals(value, dictionary)) return;
                    dictionary = (MemberChangeNotifyingDictionary<string, Bar>)value;
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