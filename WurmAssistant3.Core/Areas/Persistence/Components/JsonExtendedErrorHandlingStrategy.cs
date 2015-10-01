using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence.Components
{
    class JsonExtendedErrorHandlingStrategy : JsonDefaultErrorHandlingStrategy
    {
        readonly ILogger logger;

        public JsonExtendedErrorHandlingStrategy([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public override void HandleOnDeserialize(object o, ErrorEventArgs args)
        {
            base.HandleOnDeserialize(o, args);
            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Deserialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
        }

        public override void HandleOnSerialize(object o, ErrorEventArgs args)
        {
            base.HandleOnSerialize(o, args);
            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Serialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
        }
    }
}
