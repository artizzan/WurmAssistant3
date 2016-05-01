using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AldursLab.PersistentObjects.Serialization
{
    public class JsonDefaultErrorHandlingStrategy : IJsonErrorHandlingStrategy
    {
        public virtual void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
        {
        }

        public virtual void HandleErrorOnSerialize(object o, ErrorEventArgs args)
        {
        }

        public virtual PreviewResult PreviewJsonStringOnPopulate(string rawJson, object populatedObject)
        {
            return new PreviewResult();
        }
    }

    public class PreviewResult
    {
        public bool BreakPopulating { get; set; }
    }
}
