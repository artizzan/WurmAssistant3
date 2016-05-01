using System.Diagnostics;
using AldursLab.PersistentObjects.Serialization;
using Newtonsoft.Json.Serialization;

namespace AldursLab.PersistentObjects.Tests
{
    class TraceableErrorHandlingStrategy : JsonDefaultErrorHandlingStrategy
    {
        public override void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
        {
            Trace.WriteLine(args.ErrorContext.Error);
            base.HandleErrorOnDeserialize(o, args);
        }

        public override void HandleErrorOnSerialize(object o, ErrorEventArgs args)
        {
            Trace.WriteLine(args.ErrorContext.Error);
            base.HandleErrorOnSerialize(o, args);
        }
    }
}