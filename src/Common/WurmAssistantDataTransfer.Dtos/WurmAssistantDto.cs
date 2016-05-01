using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WurmAssistantDataTransfer.Dtos
{
    public class WurmAssistantDto
    {
        private DataSource dataSourceEnum;

        public DataSource DataSourceEnum
        {
            get { return dataSourceEnum; }
            set
            {
                dataSourceEnum = value;
                DataSourceString = value.ToString();
            }
        }

        public string DataSourceString { get; set; }
        public int Version { get; set; }

        public WurmAssistantDto()
        {
            DataSourceEnum = DataSource.Unspecified;
            Version = 0;

            Sounds = new List<Sound>();
            LegacyCustomTimerDefinitions = new List<LegacyCustomTimerDefinition>();
            Timers = new List<Timer>();
            Triggers = new List<Trigger>();
            Creatures = new List<Creature>();
        }

        public List<Sound> Sounds { get; set; }
        public List<LegacyCustomTimerDefinition> LegacyCustomTimerDefinitions { get; set; }
        public List<Timer> Timers { get; set; }
        public List<Trigger> Triggers { get; set; }
        public List<Creature> Creatures { get; set; }
    }

    public enum DataSource
    {
        Unspecified = 0,
        WurmAssistant2,
        WurmAssistant3
    }
}
