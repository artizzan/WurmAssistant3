using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Persistence.Simple.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace AldursLab.Persistence.Simple.Tests
{
    public class PersistentContextTests : TestsBase
    {
        [SetUp]
        public new void Setup()
        {
            var context = CreateContext();

            context.O1.Data = "oid1_data";
            context.O1.SampleNestedObject = new SampleNestedObject() { Data = "noid1_data" };
            context.O2.Data = "oid2_data";
            context.O2.SampleNestedObject = new SampleNestedObject() { Data = "noid1_data" };

            context.ONoNotify.Data = "ononotify_data";

            context.SaveAll();
        }

        public class SaveChangedTests : PersistentContextTests
        {
            [Test]
            public void SavesOnlyChanged()
            {
                {
                    var context = CreateContext();
                    context.O1.Data = "foo";
                    context.O1.SampleNestedObject.Data = "bar";
                    context.ONoNotify.Data = "new very important value!";
                    context.SaveChanged();
                }
                {
                    var context = CreateContext();
                    context.O1.Data.Should().Be("foo");
                    context.O1.SampleNestedObject.Data.Should().Be("bar");
                    context.O2.Data.Should().Be("oid2_data");
                    context.ONoNotify.Data.Should().Be("ononotify_data");
                }
            }
        }

        public class SaveAllTests : PersistentContextTests
        {
            [Test]
            public void SavesAll()
            {
                {
                    var context = CreateContext();
                    context.O1.Data = "foo";
                    context.O1.SampleNestedObject.Data = "bar";
                    context.ONoNotify.Data = "new very important value!";
                    context.SaveAll();
                }
                {
                    var context = CreateContext();
                    context.O1.Data.Should().Be("foo");
                    context.O1.SampleNestedObject.Data.Should().Be("bar");
                    context.O2.Data.Should().Be("oid2_data");
                    context.ONoNotify.Data.Should().Be("new very important value!");
                }
            }
        }

        public class GetOrCreateObjectSetTests : PersistentContextTests
        {
            [Test]
            public void ReturnsObjectSet()
            {
                var context = CreateContext();
                var set = context.GetOrCreateObjectSet("sample");
                set.Should().NotBeNull();
                set.Id.Should().Be("sample");
            }

            [Test]
            public void ValidatesSetId()
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
                var context = CreateContext();
                context.Invoking(sampleContext => sampleContext.GetOrCreateObjectSet(id))
                    .ShouldThrow<InvalidOperationException>();
            }

            void ShouldNotFailFor(string id)
            {
                var context = CreateContext();
                context.Invoking(sampleContext => sampleContext.GetOrCreateObjectSet(id))
                    .ShouldNotThrow<Exception>();
            }

            [Test]
            public void ObjectSetsShouldBeCaseInsensitive()
            {
                var context = CreateContext();
                var o1 = context.GetOrCreateObjectSet("ObjectSetsShouldBeCaseInsensitive");
                var o2 = context.GetOrCreateObjectSet("objectsetsshouldbecaseinsensitive");
                var o3 = context.GetOrCreateObjectSet("something different");
                o1.Should().BeSameAs(o2);
                o3.Should().NotBeSameAs(o1);
                o3.Should().NotBeSameAs(o2);
            }
        }

        public class GetOrCreateObjectSetGenericTests : PersistentContextTests
        {
            [Test]
            public void ReturnsObjectSet()
            {
                var context = CreateContext();
                var set = context.GetOrCreateObjectSet<SampleObject>("sample");
                set.Should().NotBeNull();
                set.Id.Should().Be("sample");
            }

            [Test]
            public void ValidatesSetId()
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
                var context = CreateContext();
                context.Invoking(sampleContext => sampleContext.GetOrCreateObjectSet<SampleObject>(id))
                    .ShouldThrow<InvalidOperationException>();
            }

            void ShouldNotFailFor(string id)
            {
                var context = CreateContext();
                context.Invoking(sampleContext => sampleContext.GetOrCreateObjectSet<SampleObject>(id))
                    .ShouldNotThrow<Exception>();
            }

            [Test]
            public void ObjectSetsShouldBeCaseInsensitive()
            {
                var context = CreateContext();
                var o1 = context.GetOrCreateObjectSet<SampleObject>("ObjectSetsShouldBeCaseInsensitive");
                var o2 = context.GetOrCreateObjectSet<SampleObject>("objectsetsshouldbecaseinsensitive");
                var o3 = context.GetOrCreateObjectSet<SampleObject>("something different");
                o1.GetOrCreate("1").Should().BeSameAs(o2.GetOrCreate("1"));
                o3.GetOrCreate("1").Should().NotBeSameAs(o1.GetOrCreate("1"));
                o3.GetOrCreate("1").Should().NotBeSameAs(o2.GetOrCreate("1"));
            }
        }

        public class DeleteObjectSetTests : PersistentContextTests
        {
            [Test]
            public void DeletesTheSetWithData()
            {
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet("DeleteObjectSetTests");
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data = "123123";
                    context.SaveAll();
                }
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet("DeleteObjectSetTests");
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data.Should().Be("123123");
                    context.DeleteObjectSet("DeleteObjectSetTests");
                }
                {
                    var context = CreateContext();
                    var set = context.GetOrCreateObjectSet("DeleteObjectSetTests");
                    var o = set.GetOrCreate<SampleObject>("1");
                    o.Data.Should().BeNull();
                }
            }
        }
    }
}
