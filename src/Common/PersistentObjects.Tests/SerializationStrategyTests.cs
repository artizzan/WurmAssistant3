using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace AldursLab.PersistentObjects.Tests
{
    class SerializationStrategyTests : AssertionHelper
    {
        readonly JsonSerializationStrategy strategy = new JsonSerializationStrategy();

        [SetUp]
        public void Setup()
        {
            strategy.ErrorStrategy = new JsonDefaultErrorHandlingStrategy();
        }

        [Test]
        public void WhenUnparseableValue_Throws()
        {
            string s;
            {
                T1 t1 = new T1();
                t1.D1 = "123";
                t1.D2 = true;
                s = strategy.Serialize(t1);
            }
            s = s.Replace(@"""d2"": true", @"""d2"": ""123""");
            {
                T1 t1 = new T1();
                Assert.Throws<JsonReaderException>(() => strategy.PopulateFromSerialized(t1, s));
            }
        }

        [Test]
        public void GivenErrorHandlingStrategy_WhenUnparseableValue_CanBeIgnored()
        {
            CustomizableErrorHandlingStrategy errorStrategy = new CustomizableErrorHandlingStrategy
            {
                DeserializeErrorAction = (o, args) => args.ErrorContext.Handled = true,
                SerializeErrorAction = (o, args) => args.ErrorContext.Handled = true
            };
            strategy.ErrorStrategy = errorStrategy;

            string s;
            {
                T1 t1 = new T1();
                t1.D1 = "123";
                t1.D2 = true;
                s = strategy.Serialize(t1);
            }
            s = s.Replace(@"""d2"": true", @"""d2"": ""123""");
            {
                T1 t1 = new T1();
                strategy.PopulateFromSerialized(t1, s);
                Expect(t1.D1, EqualTo("123"));
                Expect(t1.D2, False);
            }
        }

        [Test]
        public void GivenErrorHandlingStrategy_WhenBreakPopulate_Breaks()
        {
            CustomizableErrorHandlingStrategy errorStrategy = new CustomizableErrorHandlingStrategy
            {
                PrePopulateAction = (json, o) => json.Contains("test")
                    ? new PreviewResult() {BreakPopulating = true}
                    : new PreviewResult() {BreakPopulating = false}
            };
            strategy.ErrorStrategy = errorStrategy;

            {
                string s = "test, this is an invalid json that should throw exception, unless populating is skipped";
                {
                    T1 t1 = new T1();
                    strategy.PopulateFromSerialized(t1, s);
                    Expect(t1.D1, Null);
                    Expect(t1.D2, False);
                }
            }

            {
                string s = "this is also invalid json, but doesn't match skip condition, thus should cause exception";
                T1 t1 = new T1();
                Assert.Throws<JsonReaderException>(() => strategy.PopulateFromSerialized(t1, s));
            }
        }

        class CustomizableErrorHandlingStrategy : IJsonErrorHandlingStrategy
        {
            public CustomizableErrorHandlingStrategy()
            {
                DeserializeErrorAction = (o, args) => { };
                SerializeErrorAction = (o, args) => { };
                PrePopulateAction = (json, o) => new PreviewResult();
            }

            public Action<object, ErrorEventArgs> DeserializeErrorAction { get; set; }
            public Action<object, ErrorEventArgs> SerializeErrorAction { get; set; }
            public Func<string, object, PreviewResult> PrePopulateAction { get; set; }

            public void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
            {
                DeserializeErrorAction(o, args);
            }

            public void HandleErrorOnSerialize(object o, ErrorEventArgs args)
            {
                SerializeErrorAction(o, args);
            }

            public PreviewResult PreviewJsonStringOnPopulate(string rawJson, object populatedObject)
            {
                return PrePopulateAction(rawJson, populatedObject);
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        class T1
        {
            [JsonProperty]
            string[] strings;
            [JsonProperty]
            string d1;
            [JsonProperty]
            bool d2;

            public string[] Strings
            {
                get { return strings; }
                set { strings = value; }
            }

            public string D1
            {
                get { return d1; }
                set { d1 = value; }
            }

            public bool D2
            {
                get { return d2; }
                set { d2 = value; }
            }
        }
    }
}
