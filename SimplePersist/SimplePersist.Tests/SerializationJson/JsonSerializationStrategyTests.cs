using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.SimplePersist;
using AldurSoft.SimplePersist.Serializers.JsonNet;

using Xunit;

namespace SimplePersist.Tests.SerializationJson
{
    public class JsonSerializationStrategyTests
    {
        readonly JsonSerializationStrategy strategy = new JsonSerializationStrategy();
        public static readonly DateTimeOffset DtOffset = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.FromHours(1));

        [Fact]
        public void SerializesAndDeserializes()
        {
            var before = new Stringy();
            var after = Reserialize<Stringy, Stringy>(before).DeserializedEntity;
            Assert.Equal(before.String1, after.String1);
            Assert.Equal(before.DateTimeOffset, after.DateTimeOffset);
            Assert.NotNull(after.List);
        }

        [Fact]
        public void WorksForDifferentTypes()
        {
            var before = new Stringy() { List = new List<string>() { "list1" }, String1 = "abc" };
            var after = Reserialize<Stringy, StringyClone>(before).DeserializedEntity;
            Assert.Equal(before.String1, after.String1);
            Assert.Equal(before.List.Single(), after.List.Single());
            Assert.Equal(before.DateTimeOffset, after.DateTimeOffset);
        }

        [Fact]
        public void GracefullyHandlesNewField()
        {
            var before = new Stringy() { List = new List<string>() { "list1" }, String1 = "abc" };
            var after = Reserialize<Stringy, StringyWithNewField>(before).DeserializedEntity;
            Assert.Equal(before.String1, after.String1);
            Assert.Equal(before.List.Single(), after.List.Single());
            Assert.Equal(1337, after.HelloImNewHere);
        }

        [Fact]
        public void GracefullyHandlesOldField()
        {
            var before = new Stringy() { List = new List<string>() { "list1" }, String1 = "abc" };
            var after = Reserialize<Stringy, StringyWithoutAField>(before).DeserializedEntity;
            Assert.Equal(before.String1, after.String1);
            Assert.Equal(before.List.Single(), after.List.Single());
        }

        [Fact]
        public void GracefullyHandlesChangedField()
        {
            var before = new Stringy();
            var result = Reserialize<Stringy, StringyWithChangedField>(before);
            Assert.NotNull(result.SerializationErrors);
            var after = result.DeserializedEntity;
            Assert.Equal(DtOffset.DateTime, after.String1);
            Assert.Equal(before.DateTimeOffset, after.DateTimeOffset);
            Assert.NotNull(after.List);

        }

        [Fact]
        public void GracefullyHandlesMalformedData()
        {
            const string BadData = "f3egergfeW$%BY^UY%$eygwefh78dgtewf";
            var result = strategy.Deserialize<Stringy>(BadData);
            Assert.NotNull(result.SerializationErrors);
            var after = result.DeserializedEntity;
            Assert.Equal("String1", after.String1);
            Assert.Equal(DtOffset, after.DateTimeOffset);
            Assert.NotNull(after.List);
        }

            
        [Fact]
        public void PreservesNullValues()
        {
            var before = new Stringy() { String1 = null };
            var result = Reserialize<Stringy, Stringy>(before);
            Assert.Null(result.DeserializedEntity.String1);
        }


        private SerializationResult<TDestination> Reserialize<TSource, TDestination>(TSource source)
            where TSource : class, new()
            where TDestination : class, new()
        {
            var data = strategy.Serialize(source);
            var result = strategy.Deserialize<TDestination>(data);
            Trace.WriteLine(result);
            return result;
        }
    }

    class Stringy
    {
        public Stringy()
        {
            List = new List<string>();
            String1 = "String1";
            DateTimeOffset = JsonSerializationStrategyTests.DtOffset;
        }

        public string String1 { get; set; }

        public List<string> List { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
    }

    class StringyClone
    {
        public StringyClone()
        {
            List = new List<string>();
            String1 = "String1";
        }

        public string String1 { get; set; }

        public List<string> List { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
    }

    class StringyWithNewField
    {
        public StringyWithNewField()
        {
            List = new List<string>();
            String1 = "String1";
            DateTimeOffset = JsonSerializationStrategyTests.DtOffset;
            HelloImNewHere = 1337;
        }

        public string String1 { get; set; }

        public List<string> List { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public int HelloImNewHere { get; set; }
    }

    class StringyWithoutAField
    {
        public StringyWithoutAField()
        {
            List = new List<string>();
            String1 = "String1";
        }

        public string String1 { get; set; }

        public List<string> List { get; set; }
    }

    class StringyWithChangedField
    {
        public StringyWithChangedField()
        {
            List = new List<string>();
            String1 = JsonSerializationStrategyTests.DtOffset.DateTime;
            DateTimeOffset = JsonSerializationStrategyTests.DtOffset;
        }

        public DateTime String1 { get; set; }

        public List<string> List { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
    }
}
