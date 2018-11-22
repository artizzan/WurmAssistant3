using System;
using AldursLab.Persistence.Simple.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace AldursLab.Persistence.Simple.Tests
{
    public class ObjectSetTests : TestsBase
    {
        public class GetOrCreateTests : ObjectSetTests
        {
            [Test]
            public void ShouldReturnExistingObjectWhenExists()
            {
                var context = CreateContext();
                var set = context.GetOrCreateObjectSet("DeleteObjectSetTests");
                var o1 = set.GetOrCreate<SampleObject>("1");
                var o2 = set.GetOrCreate<SampleObject>("1");
                o1.Data = "123123";
                o1.Should().BeSameAs(o2);
                o2.Data.Should().Be(o1.Data);
                context.SaveAll();
            }

            [Test]
            public void ValidatesObjectId()
            {
                ShouldFailFor("this name is a little too long and should fail validation");
                ShouldFailFor("invalid characters like #$%%@");
                ShouldFailFor(null);
                ShouldFailFor(string.Empty);
                ShouldFailFor(" whitespace on left side");
                ShouldFailFor("whitespace on right side ");

                ShouldNotFailFor("dashes-");
                ShouldNotFailFor("underscores_");
                ShouldNotFailFor("numbers 123");
                ShouldNotFailFor("123 numbers on the beginning");
                ShouldNotFailFor("Upper Cased");
            }

            void ShouldFailFor(string id)
            {
                var setId = Guid.NewGuid().ToString();
                var context = CreateContext();
                var set = context.GetOrCreateObjectSet(setId);

                set.Invoking(objectSet => objectSet.GetOrCreate<SampleObject>(id))
                   .ShouldThrow<InvalidOperationException>();
            }

            void ShouldNotFailFor(string id)
            {
                var setId = Guid.NewGuid().ToString();
                var context = CreateContext();
                var set = context.GetOrCreateObjectSet(setId);

                set.Invoking(objectSet => objectSet.GetOrCreate<SampleObject>(id))
                   .ShouldNotThrow<Exception>();
            }
        }

        public class DeleteObjectTests : ObjectSetTests
        {
            [Test]
            public void DeletesTheObjectWithData()
            {
                var setId = Guid.NewGuid().ToString();
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet(setId);
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data = "123123";
                    context.SaveAll();
                }
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet(setId);
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data.Should().Be("123123");
                    set.Delete("1");
                }
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet(setId);
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data.Should().BeNull();
                }
            }
        }
    }
}