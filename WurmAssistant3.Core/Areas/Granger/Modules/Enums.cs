using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.LogFeedManager;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    public enum HorseColorId { Unknown = 0, Black = 1, White = 2, Grey = 3, Brown = 4, Gold = 5 }

    /// <summary>
    /// equal and operator overloaded to compare underlying enum
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HorseColor
    {
        [JsonProperty] 
        readonly HorseColorId horseColorId;

        public HorseColor(HorseColorId horseColorId)
        {
            this.horseColorId = horseColorId;
        }

        public HorseColor(string DBValue)
        {
            if (string.IsNullOrEmpty(DBValue)) horseColorId = HorseColorId.Unknown;
            else horseColorId = (HorseColorId)int.Parse(DBValue);
        }

        public HorseColorId HorseColorId
        {
            get { return horseColorId; }
        }

        public static HorseColor GetDefaultColor()
        {
            return new HorseColor(HorseColorId.Unknown);
        }

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(HorseColorId));
        }

        public static IEnumerable<HorseColor> GetAll()
        {
            return Enum.GetValues(typeof (HorseColorId)).Cast<HorseColorId>().Select(x => new HorseColor(x));
        }

        public override string ToString()
        {
            return horseColorId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)horseColorId).ToString();
        }

        public bool Equals(HorseColor other)
        {
            return horseColorId.Equals(other.horseColorId);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HorseColor)) return false;
            HorseColor other = (HorseColor)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return horseColorId.GetHashCode();
        }

        public static bool operator ==(HorseColor left, HorseColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HorseColor left, HorseColor right)
        {
            return !(left == right);
        }

        internal static HorseColor CreateColorFromEnumString(string enumStr)
        {
            try
            {
                return new HorseColor((HorseColorId)Enum.Parse(typeof(HorseColorId), enumStr));
            }
            catch (Exception _e)
            {
                //Aldurcraft.Utility.Logger.LogError("Parse error for HorseColor from enumStr: " + enumStr, "HorseColor", _e);
                return new HorseColor(HorseColorId.Unknown);
            }
        }

        internal static string GetDefaultColorStr()
        {
            return Enum.GetName(typeof(HorseColorId), HorseColorId.Unknown);
        }

        internal System.Drawing.Color? ToSystemDrawingColor()
        {
            switch (horseColorId)
            {
                case HorseColorId.Unknown:
                    return null;
                case HorseColorId.White:
                    return System.Drawing.Color.GhostWhite;
                case HorseColorId.Black:
                    return System.Drawing.Color.DarkSlateGray;
                case HorseColorId.Brown:
                    return System.Drawing.Color.Brown;
                case HorseColorId.Gold:
                    return System.Drawing.Color.Gold;
                case HorseColorId.Grey:
                    return System.Drawing.Color.LightGray;
                default:
                    //Aldurcraft.Utility.Logger.LogError("no ARGB match for HorseColor: " + EnumVal, this);
                    return null;
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class HorseAge : IComparable, IComparable<HorseAge>
    {
        //enum int value is used for comparable, this is limited design but whatever

        static readonly Dictionary<string, HorseAge> WurmStringToAgeMap = new Dictionary<string, HorseAge>();

        [JsonProperty]
        HorseAgeId horseAgeId;

        static HorseAge()
        {
            WurmStringToAgeMap.Add(GrangerHelpers.YOUNG.ToUpperInvariant(), new HorseAge(HorseAgeId.Young));
            WurmStringToAgeMap.Add(GrangerHelpers.ADOLESCENT.ToUpperInvariant(), new HorseAge(HorseAgeId.Adolescent));
            WurmStringToAgeMap.Add(GrangerHelpers.MATURE.ToUpperInvariant(), new HorseAge(HorseAgeId.Mature));
            WurmStringToAgeMap.Add(GrangerHelpers.AGED.ToUpperInvariant(), new HorseAge(HorseAgeId.Aged));
            WurmStringToAgeMap.Add(GrangerHelpers.OLD.ToUpperInvariant(), new HorseAge(HorseAgeId.Old));
            WurmStringToAgeMap.Add(GrangerHelpers.VENERABLE.ToUpperInvariant(), new HorseAge(HorseAgeId.Venerable));
        }

        public HorseAge(HorseAgeId horseAgeId)
        {
            this.horseAgeId = horseAgeId;
        }

        public HorseAge(string DBValue)
        {
            if (string.IsNullOrEmpty(DBValue)) horseAgeId = HorseAgeId.Unknown;
            else horseAgeId = (HorseAgeId)int.Parse(DBValue);
        }

        public HorseAgeId HorseAgeId
        {
            get { return horseAgeId; }
        }

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(HorseAgeId));
        }

        public override string ToString()
        {
            return horseAgeId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)horseAgeId).ToString();
        }

        public bool Equals(HorseAge other)
        {
            return horseAgeId == other.horseAgeId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HorseAge)) return false;
            HorseAge other = (HorseAge)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            //todo: remove mutable field as hashcode, not touching for now
            return (int)horseAgeId;
        }

        internal static HorseAge CreateAgeFromEnumString(string enumStr)
        {
            try
            {
                return new HorseAge((HorseAgeId)Enum.Parse(typeof(HorseAgeId), enumStr));
            }
            catch (Exception _e)
            {
                //Aldurcraft.Utility.Logger.LogError("Parse error for HorseAge from enumStr: " + enumStr, "HorseColor", _e);
                return new HorseAge(HorseAgeId.Unknown);
            }
        }

        internal static string GetDefaultAgeStr()
        {
            return Enum.GetName(typeof(HorseAgeId), HorseAgeId.Unknown);
        }

        internal static HorseAge CreateAgeFromRawHorseName(string objectname)
        {
            objectname = objectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.HorseAgesUpcase)
            {
                if (objectname.Contains(agestring))
                {
                    return WurmStringToAgeMap[agestring];
                }
            }

            return new HorseAge(HorseAgeId.Unknown);
        }

        internal static HorseAge CreateAgeFromRawHorseNameStartsWith(string prefixedobjectname)
        {
            prefixedobjectname = prefixedobjectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.HorseAgesUpcase)
            {
                if (prefixedobjectname.StartsWith(agestring, StringComparison.InvariantCulture))
                {
                    return WurmStringToAgeMap[agestring];
                }
            }

            return new HorseAge(HorseAgeId.Unknown);
        }

        public void Foalize()
        {
            if (this.horseAgeId == HorseAgeId.Young)
                this.horseAgeId = HorseAgeId.YoungFoal;
            else if (this.horseAgeId == HorseAgeId.Adolescent)
                this.horseAgeId = HorseAgeId.AdolescentFoal;
            else throw new InvalidOperationException("foals do not come with this age type! " + this.horseAgeId.ToString());
        }

        public int CompareTo(HorseAge other)
        {
            return ((int)horseAgeId).CompareTo((int)other.horseAgeId);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is HorseAge)
            {
                HorseAge other = (HorseAge)obj;
                return CompareTo(other);
            }
            else throw new ArgumentException("Object is not a HorseAge");
        }

        public static bool operator <(HorseAge left, HorseAge right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(HorseAge left, HorseAge right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator <=(HorseAge left, HorseAge right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >=(HorseAge left, HorseAge right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(HorseAge left, HorseAge right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(HorseAge left, HorseAge right)
        {
            return !(left == right);
        }
    }

    public enum HorseAgeId : int
    { 
        Unknown=0, YoungFoal=100, AdolescentFoal=200, Young=300, Adolescent=400, Mature=500, Aged=600, Old=700, Venerable=800 
    }
}
