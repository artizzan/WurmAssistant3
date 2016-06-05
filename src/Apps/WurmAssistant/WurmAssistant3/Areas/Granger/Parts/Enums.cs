using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.LogFeedManager;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts
{
    public enum CreatureColorId
    {
        Unknown = 0, Black = 1, White = 2, Grey = 3, Brown = 4,
        Gold = 5, BloodBay = 6, EbonyBlack = 7, PiebaldPinto = 8
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureColor
    {
        [JsonProperty] 
        readonly CreatureColorId creatureColorId;

        public CreatureColor(CreatureColorId creatureColorId)
        {
            this.creatureColorId = creatureColorId;
        }

        public CreatureColor(string DBValue)
        {
            if (string.IsNullOrEmpty(DBValue)) creatureColorId = CreatureColorId.Unknown;
            else creatureColorId = (CreatureColorId)int.Parse(DBValue);
        }

        public CreatureColorId CreatureColorId
        {
            get { return creatureColorId; }
        }

        public static CreatureColor GetDefaultColor()
        {
            return new CreatureColor(CreatureColorId.Unknown);
        }

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(CreatureColorId));
        }

        public static IEnumerable<CreatureColor> GetAll()
        {
            return Enum.GetValues(typeof (CreatureColorId)).Cast<CreatureColorId>().Select(x => new CreatureColor(x));
        }

        public override string ToString()
        {
            return creatureColorId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)creatureColorId).ToString();
        }

        public bool Equals(CreatureColor other)
        {
            return creatureColorId.Equals(other.creatureColorId);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CreatureColor)) return false;
            CreatureColor other = (CreatureColor)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return creatureColorId.GetHashCode();
        }

        public static bool operator ==(CreatureColor left, CreatureColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CreatureColor left, CreatureColor right)
        {
            return !(left == right);
        }

        internal static CreatureColor CreateColorFromEnumString(string enumStr)
        {
            try
            {
                return new CreatureColor((CreatureColorId)Enum.Parse(typeof(CreatureColorId), enumStr, true));
            }
            catch (Exception)
            {
                return new CreatureColor(CreatureColorId.Unknown);
            }
        }

        internal static string GetDefaultColorStr()
        {
            return Enum.GetName(typeof(CreatureColorId), CreatureColorId.Unknown);
        }

        internal System.Drawing.Color? ToSystemDrawingColor()
        {
            switch (creatureColorId)
            {
                case CreatureColorId.Unknown:
                    return null;
                case CreatureColorId.White:
                    return System.Drawing.Color.GhostWhite;
                case CreatureColorId.Black:
                    return System.Drawing.Color.DarkSlateGray;
                case CreatureColorId.Brown:
                    return System.Drawing.Color.Brown;
                case CreatureColorId.Gold:
                    return System.Drawing.Color.Gold;
                case CreatureColorId.Grey:
                    return System.Drawing.Color.LightGray;
                case CreatureColorId.BloodBay:
                    return System.Drawing.Color.RosyBrown;
                case CreatureColorId.EbonyBlack:
                    return System.Drawing.Color.Black;
                case CreatureColorId.PiebaldPinto:
                    return System.Drawing.Color.DarkGray;
                default:
                    return null;
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureAge : IComparable, IComparable<CreatureAge>
    {
        //enum int value is used for comparable, this is limited design but whatever

        static readonly Dictionary<string, CreatureAge> WurmStringToAgeMap = new Dictionary<string, CreatureAge>();

        [JsonProperty]
        CreatureAgeId creatureAgeId;

        static CreatureAge()
        {
            WurmStringToAgeMap.Add(GrangerHelpers.YOUNG.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Young));
            WurmStringToAgeMap.Add(GrangerHelpers.ADOLESCENT.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Adolescent));
            WurmStringToAgeMap.Add(GrangerHelpers.MATURE.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Mature));
            WurmStringToAgeMap.Add(GrangerHelpers.AGED.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Aged));
            WurmStringToAgeMap.Add(GrangerHelpers.OLD.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Old));
            WurmStringToAgeMap.Add(GrangerHelpers.VENERABLE.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Venerable));
        }

        public CreatureAge(CreatureAgeId creatureAgeId)
        {
            this.creatureAgeId = creatureAgeId;
        }

        public CreatureAge(string DBValue)
        {
            if (string.IsNullOrEmpty(DBValue)) creatureAgeId = CreatureAgeId.Unknown;
            else creatureAgeId = (CreatureAgeId)int.Parse(DBValue);
        }

        public CreatureAgeId CreatureAgeId
        {
            get { return creatureAgeId; }
        }

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(CreatureAgeId));
        }

        public override string ToString()
        {
            return creatureAgeId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)creatureAgeId).ToString();
        }

        public bool Equals(CreatureAge other)
        {
            return creatureAgeId == other.creatureAgeId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CreatureAge)) return false;
            CreatureAge other = (CreatureAge)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            //todo: remove mutable field as hashcode, not touching for now
            return (int)creatureAgeId;
        }

        internal static CreatureAge CreateAgeFromEnumString(string enumStr)
        {
            try
            {
                return new CreatureAge((CreatureAgeId)Enum.Parse(typeof(CreatureAgeId), enumStr, true));
            }
            catch (Exception)
            {
                return new CreatureAge(CreatureAgeId.Unknown);
            }
        }

        internal static string GetDefaultAgeStr()
        {
            return Enum.GetName(typeof(CreatureAgeId), CreatureAgeId.Unknown);
        }

        internal static CreatureAge CreateAgeFromRawCreatureName(string objectname)
        {
            objectname = objectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.CreatureAgesUpcase)
            {
                if (objectname.Contains(agestring))
                {
                    return WurmStringToAgeMap[agestring].CreateCopy();
                }
            }

            return new CreatureAge(CreatureAgeId.Unknown);
        }

        internal static CreatureAge CreateAgeFromRawCreatureNameStartsWith(string prefixedobjectname)
        {
            prefixedobjectname = prefixedobjectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.CreatureAgesUpcase)
            {
                if (prefixedobjectname.StartsWith(agestring, StringComparison.InvariantCultureIgnoreCase))
                {
                    return WurmStringToAgeMap[agestring].CreateCopy();
                }
            }

            return new CreatureAge(CreatureAgeId.Unknown);
        }

        CreatureAge CreateCopy()
        {
            return new CreatureAge(this.creatureAgeId);
        }

        public void Foalize()
        {
            if (this.creatureAgeId == CreatureAgeId.Young)
                this.creatureAgeId = CreatureAgeId.YoungFoal;
            else if (this.creatureAgeId == CreatureAgeId.Adolescent)
                this.creatureAgeId = CreatureAgeId.AdolescentFoal;
            else throw new InvalidOperationException("foals do not come with this age type! " + this.creatureAgeId.ToString());
        }

        public int CompareTo(CreatureAge other)
        {
            return ((int)creatureAgeId).CompareTo((int)other.creatureAgeId);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is CreatureAge)
            {
                CreatureAge other = (CreatureAge)obj;
                return CompareTo(other);
            }
            else throw new ArgumentException("Object is not a CreatureAge");
        }

        public static bool operator <(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator <=(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >=(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) >= 0;
        }
        public static bool operator ==(CreatureAge left, CreatureAge right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(CreatureAge left, CreatureAge right)
        {
            return !(left == right);
        }
    }

    public enum CreatureAgeId : int
    { 
        Unknown=0, YoungFoal=100, AdolescentFoal=200, Young=300, Adolescent=400, Mature=500, Aged=600, Old=700, Venerable=800 
    }
}
