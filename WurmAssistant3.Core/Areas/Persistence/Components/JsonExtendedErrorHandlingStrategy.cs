using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Views;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence.Components
{
    class JsonExtendedErrorHandlingStrategy : JsonDefaultErrorHandlingStrategy
    {
        readonly ILogger logger;
        readonly IHostEnvironment host;

        public JsonExtendedErrorHandlingStrategy([NotNull] ILogger logger, [NotNull] IHostEnvironment host)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (host == null) throw new ArgumentNullException("host");
            this.logger = logger;
            this.host = host;
        }

        public override void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
        {
            base.HandleErrorOnDeserialize(o, args);
            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Deserialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
            var view = new DeserializationErrorResolverView(o, args);
            if (view.ShowDialog() == DialogResult.No)
            {
                host.Shutdown();
                //throw new ApplicationException("test");
                
            }
        }

        public override void HandleErrorOnSerialize(object o, ErrorEventArgs args)
        {
            base.HandleErrorOnSerialize(o, args);
            logger.Error(args.ErrorContext.Error,
                string.Format((string) "Serialization error. Member = {0} ; Path = {1}",
                    args.ErrorContext.Member,
                    args.ErrorContext.Path));
        }
    }
}
