using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AldursLab.WurmApi
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public struct WurmDay : IEquatable<WurmDay>, IComparable<WurmDay>, IComparable
    {
        private const string AntNameNormalized = "ANT",
                             LuckNameNormalized = "LUCK",
                             WurmNameNormalized = "WURM",
                             WrathNameNormalized = "WRATH",
                             TearsNameNormalized = "TEARS",
                             SleepNameNormalized = "SLEEP",
                             AwakeningNameNormalized = "AWAKENING";

        private static readonly string[] WurmStarfallNames =
        {
            AntNameNormalized,
            LuckNameNormalized,
            WurmNameNormalized,
            WrathNameNormalized,
            TearsNameNormalized,
            SleepNameNormalized,
            AwakeningNameNormalized
        };

        public static IEnumerable<string> AllNormalizedNames => WurmStarfallNames;

        private readonly WurmDayId wurmDayId;

        public WurmDayId Id => wurmDayId;

        public WurmDay(int number)
            : this((WurmDayId)number)
        {
        }

        public WurmDay(WurmDayId wurmDayId)
        {
            this.wurmDayId = wurmDayId;
        }

        public WurmDay(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            name = name.ToUpperInvariant();

            switch (name)
            {
                case AntNameNormalized:
                    wurmDayId = WurmDayId.Ant;
                    break;
                case LuckNameNormalized:
                    wurmDayId = WurmDayId.Luck;
                    break;
                case WurmNameNormalized:
                    wurmDayId = WurmDayId.Wurm;
                    break;
                case WrathNameNormalized:
                    wurmDayId = WurmDayId.Wrath;
                    break;
                case TearsNameNormalized:
                    wurmDayId = WurmDayId.Tears;
                    break;
                case SleepNameNormalized:
                    wurmDayId = WurmDayId.Sleep;
                    break;
                case AwakeningNameNormalized:
                    wurmDayId = WurmDayId.Awakening;
                    break;
                default:
                    throw new WurmApiException("Invalid wurm day");
            }
        }

        public static WurmDay Ant => new WurmDay(WurmDayId.Ant);
        public static WurmDay Luck => new WurmDay(WurmDayId.Luck);
        public static WurmDay Wurm => new WurmDay(WurmDayId.Wurm);
        public static WurmDay Wrath => new WurmDay(WurmDayId.Wrath);
        public static WurmDay Tears => new WurmDay(WurmDayId.Tears);
        public static WurmDay Sleep => new WurmDay(WurmDayId.Sleep);
        public static WurmDay Awakening => new WurmDay(WurmDayId.Awakening);

        public override string ToString()
        {
            switch (wurmDayId)
            {
                case WurmDayId.Ant:
                    return "Ant";
                case WurmDayId.Luck:
                    return "Luck";
                case WurmDayId.Wurm:
                    return "Wurm";
                case WurmDayId.Wrath:
                    return "Wrath";
                case WurmDayId.Tears:
                    return "Tears";
                case WurmDayId.Sleep:
                    return "Sleep";
                case WurmDayId.Awakening:
                    return "Awakening";
                default:
                    throw new WurmApiException("Unknown wurm day id: " + wurmDayId);
            }
        }

        public bool Equals(WurmDay other)
        {
            return wurmDayId == other.wurmDayId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is WurmDayId && Equals((WurmDayId)obj);
        }

        public override int GetHashCode()
        {
            return (int)wurmDayId;
        }

        public int CompareTo(object other)
        {
            if (!(other is WurmDayId))
                return int.MinValue;
            return CompareTo((WurmDayId)other);
        }
        public int CompareTo(WurmDay other)
        {
            return wurmDayId.CompareTo(other.wurmDayId);
        }

        public static bool operator >(WurmDay left, WurmDay right)
        {
            return left.wurmDayId > right.wurmDayId;
        }

        public static bool operator <(WurmDay left, WurmDay right)
        {
            return left.wurmDayId < right.wurmDayId;
        }

        public static bool operator >=(WurmDay left, WurmDay right)
        {
            return left.wurmDayId >= right.wurmDayId;
        }

        public static bool operator <=(WurmDay left, WurmDay right)
        {
            return left.wurmDayId <= right.wurmDayId;
        }

        public static bool operator ==(WurmDay left, WurmDay right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WurmDay left, WurmDay right)
        {
            return !left.Equals(right);
        }

        public int Number => (int)wurmDayId;
    }

    public enum WurmDayId
    {
        Ant = 1,
        Luck,
        Wurm,
        Wrath,
        Tears,
        Sleep,
        Awakening
    }
}