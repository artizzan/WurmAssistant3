using System;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public struct TraitsInspectedContainer : IComparable
    {
        public float Skill;
        public bool EpicCurve;

        public override string ToString()
        {
            return EpicCurve == true ? Skill.ToString() + " (epic)" : Skill.ToString();
        }

        public int CompareTo(TraitsInspectedContainer other)
        {
            return Skill.CompareTo(other.Skill);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is TraitsInspectedContainer)
            {
                return CompareTo((TraitsInspectedContainer)obj);
            }
            else
                throw new ArgumentException("Object is not a TraitsInspectedContained");
        }
    }
}