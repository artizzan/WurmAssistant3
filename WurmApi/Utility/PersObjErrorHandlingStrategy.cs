using System;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Utility
{
    class PersObjErrorHandlingStrategy : IObjectDeserializationErrorHandlingStrategy
    {
        readonly IWurmApiLogger logger;

        public PersObjErrorHandlingStrategy([NotNull] IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        public void Handle(ErrorContext errorContext)
        {
            logger.Log(LogLevel.Error,
                "Persistent object deserialization error, will ignore and use defaults, error details: "
                + errorContext.GetErrorDetailsAsString(), "WurmApi", null);
            errorContext.Decision = Decision.IgnoreErrorsAndReturnDefaultsForMissingData;
        }
    }
}
