using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AldurSoft.WurmApi.Wurm.DateAndTime
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public struct WurmStarfall : IEquatable<WurmStarfall>, IComparable<WurmStarfall>, IComparable
    {
        private const string DiamondNameNormalized = "DIAMOND",
                             SawNameNormalized = "SAW",
                             DiggingNameNormalized = "DIGGING",
                             LeafNameNormalized = "LEAF",
                             BearNameNormalized = "BEAR",
                             SnakeNameNormalized = "SNAKE",
                             WhitesharkNameNormalized = "WHITE SHARK",
                             FireNameNormalized = "FIRE",
                             RavenNameNormalized = "RAVEN",
                             DancerNameNormalized = "DANCER",
                             OmenNameNormalized = "OMEN",
                             SilentNameNormalized = "SILENT";

        private static readonly string[] WurmStarfallNames =
        {
            DiamondNameNormalized,
            SawNameNormalized,
            DiggingNameNormalized,
            LeafNameNormalized,
            BearNameNormalized,
            SnakeNameNormalized,
            WhitesharkNameNormalized,
            FireNameNormalized,
            RavenNameNormalized,
            DancerNameNormalized,
            OmenNameNormalized,
            SilentNameNormalized
        };

        public static IEnumerable<string> AllNormalizedNames
        {
            get { return WurmStarfallNames; }
        }

        private readonly WurmStarfallId wurmStarfallId;

        public WurmStarfallId Id
        {
            get { return wurmStarfallId; }
        }

        public WurmStarfall(int number)
            : this((WurmStarfallId)number)
        {
        }

        public WurmStarfall(WurmStarfallId wurmStarfallId)
        {
            this.wurmStarfallId = wurmStarfallId;
        }

        public WurmStarfall(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            name = name.ToUpperInvariant();

            switch (name)
            {
                case DiamondNameNormalized:
                    wurmStarfallId = WurmStarfallId.Diamond;
                    break;
                case SawNameNormalized:
                    wurmStarfallId = WurmStarfallId.Saw;
                    break;
                case DiggingNameNormalized:
                    wurmStarfallId = WurmStarfallId.Digging;
                    break;
                case LeafNameNormalized:
                    wurmStarfallId = WurmStarfallId.Leaf;
                    break;
                case BearNameNormalized:
                    wurmStarfallId = WurmStarfallId.Bear;
                    break;
                case SnakeNameNormalized:
                    wurmStarfallId = WurmStarfallId.Snake;
                    break;
                case WhitesharkNameNormalized:
                    wurmStarfallId = WurmStarfallId.WhiteShark;
                    break;
                case FireNameNormalized:
                    wurmStarfallId = WurmStarfallId.Fire;
                    break;
                case RavenNameNormalized:
                    wurmStarfallId = WurmStarfallId.Raven;
                    break;
                case DancerNameNormalized:
                    wurmStarfallId = WurmStarfallId.Dancer;
                    break;
                case OmenNameNormalized:
                    wurmStarfallId = WurmStarfallId.Omen;
                    break;
                case SilentNameNormalized:
                    wurmStarfallId = WurmStarfallId.Silent;
                    break;
                default:
                    throw new WurmApiException("Invalid wurm starfall");
            }
        }

        public static WurmStarfall Diamond { get { return new WurmStarfall(WurmStarfallId.Diamond);} }
        public static WurmStarfall Saw { get { return new WurmStarfall(WurmStarfallId.Saw); } }
        public static WurmStarfall Digging { get { return new WurmStarfall(WurmStarfallId.Digging); } }
        public static WurmStarfall Leaf { get { return new WurmStarfall(WurmStarfallId.Leaf); } }
        public static WurmStarfall Bear { get { return new WurmStarfall(WurmStarfallId.Bear); } }
        public static WurmStarfall Snake { get { return new WurmStarfall(WurmStarfallId.Snake); } }
        public static WurmStarfall WhiteShark { get { return new WurmStarfall(WurmStarfallId.WhiteShark); } }
        public static WurmStarfall Fire { get { return new WurmStarfall(WurmStarfallId.Fire); } }
        public static WurmStarfall Raven { get { return new WurmStarfall(WurmStarfallId.Raven); } }
        public static WurmStarfall Dancer { get { return new WurmStarfall(WurmStarfallId.Dancer); } }
        public static WurmStarfall Omen { get { return new WurmStarfall(WurmStarfallId.Omen); } }
        public static WurmStarfall Silent { get { return new WurmStarfall(WurmStarfallId.Silent); } }

        public override string ToString()
        {
            switch (wurmStarfallId)
            {
                case WurmStarfallId.Diamond:
                    return "Diamond";
                case WurmStarfallId.Saw:
                    return "Saw";
                case WurmStarfallId.Digging:
                    return "Digging";
                case WurmStarfallId.Leaf:
                    return "Leaf";
                case WurmStarfallId.Bear:
                    return "Bear";
                case WurmStarfallId.Snake:
                    return "Snake";
                case WurmStarfallId.WhiteShark:
                    return "White Shark";
                case WurmStarfallId.Fire:
                    return "Fire";
                case WurmStarfallId.Raven:
                    return "Raven";
                case WurmStarfallId.Dancer:
                    return "Dancer";
                case WurmStarfallId.Omen:
                    return "Omen";
                case WurmStarfallId.Silent:
                    return "Silent";
                default:
                    throw new WurmApiException("Unknown wurm starfall id: " + wurmStarfallId);
            }
        }

        public bool Equals(WurmStarfall other)
        {
            return wurmStarfallId == other.wurmStarfallId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is WurmStarfall && Equals((WurmStarfall)obj);
        }

        public override int GetHashCode()
        {
            return (int)wurmStarfallId;
        }

        public int CompareTo(object other)
        {
            if (!(other is WurmStarfall)) return int.MinValue;
            return this.CompareTo((WurmStarfall)other);
        }
        public int CompareTo(WurmStarfall other)
        {
            return this.wurmStarfallId.CompareTo(other.wurmStarfallId);
        }

        public static bool operator >(WurmStarfall left, WurmStarfall right)
        {
            return left.wurmStarfallId > right.wurmStarfallId;
        }

        public static bool operator <(WurmStarfall left, WurmStarfall right)
        {
            return left.wurmStarfallId < right.wurmStarfallId;
        }

        public static bool operator >=(WurmStarfall left, WurmStarfall right)
        {
            return left.wurmStarfallId >= right.wurmStarfallId;
        }

        public static bool operator <=(WurmStarfall left, WurmStarfall right)
        {
            return left.wurmStarfallId <= right.wurmStarfallId;
        }

        public static bool operator ==(WurmStarfall left, WurmStarfall right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WurmStarfall left, WurmStarfall right)
        {
            return !left.Equals(right);
        }

        public int Number
        {
            get { return (int)wurmStarfallId; }
        }
    }

    public enum WurmStarfallId
    {
        Diamond = 1,
        Saw,
        Digging,
        Leaf,
        Bear,
        Snake,
        WhiteShark,
        Fire,
        Raven,
        Dancer,
        Omen,
        Silent
    }
}