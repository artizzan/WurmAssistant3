using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AldursLab.WurmApi
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
                             SilentNameNormalized = "SILENCE";

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

        public static IEnumerable<string> AllNormalizedNames => WurmStarfallNames;

        private readonly WurmStarfallId wurmStarfallId;

        public WurmStarfallId Id => wurmStarfallId;

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
            if (name == null) throw new ArgumentNullException(nameof(name));
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

        public static WurmStarfall Diamond => new WurmStarfall(WurmStarfallId.Diamond);
        public static WurmStarfall Saw => new WurmStarfall(WurmStarfallId.Saw);
        public static WurmStarfall Digging => new WurmStarfall(WurmStarfallId.Digging);
        public static WurmStarfall Leaf => new WurmStarfall(WurmStarfallId.Leaf);
        public static WurmStarfall Bear => new WurmStarfall(WurmStarfallId.Bear);
        public static WurmStarfall Snake => new WurmStarfall(WurmStarfallId.Snake);
        public static WurmStarfall WhiteShark => new WurmStarfall(WurmStarfallId.WhiteShark);
        public static WurmStarfall Fire => new WurmStarfall(WurmStarfallId.Fire);
        public static WurmStarfall Raven => new WurmStarfall(WurmStarfallId.Raven);
        public static WurmStarfall Dancer => new WurmStarfall(WurmStarfallId.Dancer);
        public static WurmStarfall Omen => new WurmStarfall(WurmStarfallId.Omen);
        public static WurmStarfall Silent => new WurmStarfall(WurmStarfallId.Silent);

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

        public static int GetDaysInStarfall()
        {
            return 28;
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
            return CompareTo((WurmStarfall)other);
        }
        public int CompareTo(WurmStarfall other)
        {
            return wurmStarfallId.CompareTo(other.wurmStarfallId);
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

        public int Number => (int)wurmStarfallId;
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