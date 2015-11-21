using System;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Core.Areas.RevealCreatures.Data
{
    public sealed class Direction : IEquatable<Direction>
    {
        const string AheadLeft = "ahead of you to the left";
        const string Ahead = "in front of you";
        const string AheadRight = "ahead of you to the right";
        const string Right = "to the right of you";
        const string BehindRight = "behind you to the right";
        const string Behind = "behind you";
        const string BehindLeft = "behind you to the left";
        const string Left = "to the left of you";

        public string WurmLogString { get; private set; }
        public string ShortString { get; private set; }

        public static Direction CreateFromLogEntryString(string text)
        {
            if (text.Contains(AheadLeft, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = AheadLeft,
                    ShortString = "ahead left"
                };
            }
            else if (text.Contains(Ahead, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = Ahead,
                    ShortString = "ahead"
                };
            }
            else if (text.Contains(AheadRight, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = AheadRight,
                    ShortString = "ahead right"
                };
            }
            else if (text.Contains(Right, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = Right,
                    ShortString = "right"
                };
            }
            else if (text.Contains(BehindRight, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = BehindRight,
                    ShortString = "behind right"
                };
            }
            else if (text.Contains(Behind, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = Behind,
                    ShortString = "behind"
                };
            }
            else if (text.Contains(BehindLeft, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = BehindLeft,
                    ShortString = "behind left"
                };
            }
            else if (text.Contains(Left, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Direction()
                {
                    WurmLogString = Left,
                    ShortString = "left"
                };
            }
            else
            {
                return new Direction()
                {
                    WurmLogString = "",
                    ShortString = ""
                };
            }
        }

        public override string ToString()
        {
            return ShortString;
        }


        public bool Equals(Direction other)
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
            return Equals((Direction) obj);
        }

        public override int GetHashCode()
        {
            return (WurmLogString != null ? WurmLogString.GetHashCode() : 0);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !Equals(left, right);
        }
    }
}