using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Systems.DataBackups
{
    [Serializable]
    public class DataBackupRestoreCanceledException : Exception
    {
        public DataBackupRestoreCanceledException()
        {
        }

        public DataBackupRestoreCanceledException(string message) : base(message)
        {
        }

        public DataBackupRestoreCanceledException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DataBackupRestoreCanceledException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
