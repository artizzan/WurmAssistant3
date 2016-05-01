using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class ComparableExtensions
    {
        /// <summary>
        /// If the value is less than min or more than max, returns min or max respectively, else returns the value.
        /// If the value is null, returns min value.
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <exception cref="ConstraintException">throwException is true and Value is outside constraints</exception>
        public static T ConstrainToRange<T>(this T value, [NotNull] T min, [NotNull] T max, bool throwException = false)
            where T : System.IComparable<T>
        {
            if (min == null) throw new ArgumentNullException("min");
            if (max == null) throw new ArgumentNullException("max");
            if (ReferenceEquals(value, null))
            {
                if (throwException) throw new ConstraintException("Value is null");
                return min;
            }
            if (value.CompareTo(min) < 0)
            {
                if (throwException) throw new ConstraintException("Value is less than minimum");
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                if (throwException) throw new ConstraintException("Value is more than maximum");
                return max;
            }
            else
                return value;
        }
    }

    [Serializable]
    public class ConstraintException : Exception
    {
        public ConstraintException()
        {
        }

        public ConstraintException(string message) : base(message)
        {
        }

        public ConstraintException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConstraintException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
