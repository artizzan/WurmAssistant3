using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Extensions
{
    class ComparableExtrensionsTests : AssertionHelper
    {
        public class ConstrainValueTests : AssertionHelper
        {
            class NumericComparableClass : IEquatable<NumericComparableClass>, IComparable<NumericComparableClass>
            {
                public NumericComparableClass(int value)
                {
                    Value = value;
                }

                public int Value { get; private set; }

                public bool Equals(NumericComparableClass other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return Value == other.Value;
                }

                public int CompareTo(NumericComparableClass other)
                {
                    if (ReferenceEquals(null, other)) throw new ArgumentException("other is null");
                    return Value.CompareTo(other.Value);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != this.GetType()) return false;
                    return Equals((NumericComparableClass) obj);
                }

                public override int GetHashCode()
                {
                    return Value;
                }

                public static bool operator ==(NumericComparableClass left, NumericComparableClass right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(NumericComparableClass left, NumericComparableClass right)
                {
                    return !Equals(left, right);
                }

                public override string ToString()
                {
                    return string.Format("Value: {0}", Value);
                }
            }

            NumericComparableClass min = new NumericComparableClass(3);
            NumericComparableClass max = new NumericComparableClass(8);

            [Test]
            public void WhenLessThanMin_ReturnsMin()
            {
                var value = new NumericComparableClass(1);
                var result = value.ConstrainToRange(min, max);
                Expect(result, EqualTo(min));
            }

            [Test]
            public void WhenMoreThanMax_ReturnsMax()
            {
                var value = new NumericComparableClass(10);
                var result = value.ConstrainToRange(min, max);
                Expect(result, EqualTo(max));
            }

            [Test]
            public void WhenBetweenMinMax_ReturnsValue()
            {
                var value = new NumericComparableClass(5);
                var result = value.ConstrainToRange(min, max);
                Expect(result, EqualTo(new NumericComparableClass(5)));
            }

            [Test]
            public void WhenValueNull_ReturnsMin()
            {
                NumericComparableClass value = null;
                var result = value.ConstrainToRange(min, max);
                Expect(result, EqualTo(min));
            }

            [Test]
            public void WhenMinNull_Throws()
            {
                var value = new NumericComparableClass(5);
                Assert.Throws<ArgumentNullException>(() => value.ConstrainToRange(null, max));
            }

            [Test]
            public void WhenMaxNull_Throws()
            {
                var value = new NumericComparableClass(5);
                Assert.Throws<ArgumentNullException>(() => value.ConstrainToRange(min, null));
            }

            [Test]
            public void GivenThrowException_WhenOutsideConstraints_Throws()
            {
                var value = new NumericComparableClass(1);
                Assert.Throws<ConstraintException>(() => value.ConstrainToRange(min, max, true));
                value = new NumericComparableClass(10);
                Assert.Throws<ConstraintException>(() => value.ConstrainToRange(min, max, true));
                value = null;
                Assert.Throws<ConstraintException>(() => value.ConstrainToRange(min, max, true));
            }
        }
        
    }
}
