using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AldursLab.PersistentObjects.Tests
{
    public class JsonNetTests : AssertionHelper
    {
        [Test]
        public void JsonObjectAttributeIsInherited()
        {
            var src = new DerivedSample();
            src.Data = "Data";
            var dest = Reserialize(src);
            Expect(dest.Data, EqualTo(null));
        }

        [Test]
        public void JsonObjectAttributeCanBeOverriden()
        {
            var src = new DerivedOverridingSample();
            src.Data = "Data";
            var dest = Reserialize(src);
            Expect(dest.Data, EqualTo("Data"));
        }

        T Reserialize<T>(T derivedSample)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(derivedSample));
        }


        class DerivedSample : Sample
        {
        }

        [JsonObject(MemberSerialization.OptOut)]
        class DerivedOverridingSample : Sample
        {
        }

        [JsonObject(MemberSerialization.OptIn)]
        abstract class Sample
        {
            public string Data { get; set; }
        }
    }
}
