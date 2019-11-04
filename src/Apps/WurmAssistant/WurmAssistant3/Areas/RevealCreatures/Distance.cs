using System;
using AldursLab.Essentials.Extensions.DotNet;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.RevealCreatures
{
    public sealed class Distance : IEquatable<Distance>, IComparable<Distance>, IComparable
    {
        const string PracticallyStandingOn = "practicly standing on";
        const string StoneThrowAway = "a stone's throw away";
        const string VeryCloseBy = "very close";
        const string PrettyCloseBy = "pretty close by";
        const string FairlyCloseBy = "fairly close by";
        const string SomeDistance = "some distance";
        const string QuiteSomeDistance = "quite some distance";

        public string WurmLogString { get; private set; }
        public string ShortString { get; private set; }
        public int AverageDistance { get; private set; }

        public static Distance CreateFromLogEntryString(string text)
        {
            if (text.Contains(PracticallyStandingOn, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 0,
                    ShortString = "0",
                    WurmLogString = PracticallyStandingOn
                };
            }
            else if (text.Contains(StoneThrowAway, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 2,
                    ShortString = "1-3",
                    WurmLogString = StoneThrowAway
                };
            }
            else if (text.Contains(VeryCloseBy, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 4,
                    ShortString = "4-5",
                    WurmLogString = VeryCloseBy
                };
            }
            else if (text.Contains(PrettyCloseBy, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 7,
                    ShortString = "6-9",
                    WurmLogString = PrettyCloseBy
                };
            }
            else if (text.Contains(FairlyCloseBy, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 15,
                    ShortString = "10-19",
                    WurmLogString = FairlyCloseBy
                };
            }
            else if (text.Contains(SomeDistance, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 35,
                    ShortString = "20-49",
                    WurmLogString = SomeDistance
                };
            }
            else if (text.Contains(QuiteSomeDistance, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Distance()
                {
                    AverageDistance = 65,
                    ShortString = "50-80",
                    WurmLogString = QuiteSomeDistance
                };
            }
            else
            {
                return new Distance()
                {
                    AverageDistance = int.MaxValue,
                    ShortString = "",
                    WurmLogString = ""
                };
            }
        }

        public override string ToString()
        {
            return ShortString;
        }

        public bool Equals(Distance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(WurmLogString, other.WurmLogString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Distance) obj);
        }

        public override int GetHashCode()
        {
            return (WurmLogString != null ? WurmLogString.GetHashCode() : 0);
        }

        public static bool operator ==(Distance left, Distance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Distance left, Distance right)
        {
            return !Equals(left, right);
        }

        public int CompareTo([NotNull] object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            var other = obj as Distance;
            if (other == null) throw new ArgumentException("obj is not " + typeof(Distance).FullName);
            return this.CompareTo(other);
        }

        public int CompareTo([NotNull] Distance other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return AverageDistance.CompareTo(other.AverageDistance);
        }
    }
}