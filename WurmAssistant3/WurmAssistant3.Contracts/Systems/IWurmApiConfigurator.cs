using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AldursLab.WurmAssistant3.Systems
{
    public interface IWurmApiConfigurator
    {
        void AppendTimeQueryToAutoruns();

        void AppendUptimeQueryToAutoruns();

        void SynchronizeWurmGameClientSettings();

        WurmGameClientSettingsSyncState VerifyGameClientConfig();
    }

    public class WurmGameClientSettingsSyncState
    {
        private readonly List<string> issues = new List<string>();
        public bool AnyIssues { get { return Issues.Any(); } }
        public IList<string> Issues { get { return issues; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var configProblem in issues)
            {
                sb.AppendLine(configProblem);
            }
            return sb.ToString();
        }
    }
}
