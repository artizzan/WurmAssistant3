using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Persistence;
using AldursLab.WurmAssistant.Launcher.Dto;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class GlobalSettings
    {
        public class Data
        {
            public int Version { get; set; }
            public List<string> BaseUrlPriorityList = new List<string>(); 
        }

        readonly Persistent<Data> persistent;

        public GlobalSettings(ControllerConfig config)
        {
            var filepath = Path.Combine(config.LauncherBinDirFullPath, "globalsettings.json");
            
            persistent = new Persistent<Data>(filepath);
            persistent.Load();

            if (persistent.Data.Version == 0)
            {
                persistent.Data.BaseUrlPriorityList = new List<string>()
                {
                    "http://wurmassistant.azurewebsites.net/api/WurmAssistant3",
                    "http://wurmassistant.aldurcraft.net/api/WurmAssistant3",
                    "http://wurmassistant.aldurslab.net/api/WurmAssistant3"
                };
                persistent.Data.Version = 1;
                persistent.Save();
            }
        }

        public IEnumerable<string> BaseUrlPriorityList
        {
            get
            {
                return persistent.Data.BaseUrlPriorityList.ToArray();
            }
            set
            {
                persistent.Data.BaseUrlPriorityList = new List<string>(value);
                persistent.Save();
            }
        }
    }
}
