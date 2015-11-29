using System.IO;
using AldursLab.Persistence;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class UserSettings
    {
        public class Data
        {
            public Data()
            {
                WurmAssistantWebServiceUrl = "http://wurmassistant.azurewebsites.net/api/WurmAssistant3";
            }

            public bool UseRelativeDataDir { get; set; }
            public string SpecificBuildNumber { get; set; }
            public LaunchChoices LastLaunchChoice { get; set; }
            public bool UpdateSourceEstablished { get; set; }
            public string WurmAssistantWebServiceUrl { get; set; }
        }

        readonly Persistent<Data> persistent;

        public UserSettings(string dirpath)
        {
            var filepath = Path.Combine(dirpath, "usersettings.json");
            
            persistent = new Persistent<Data>(filepath);
            persistent.Load();
        }

        public bool UseRelativeDataDir
        {
            get
            {
                return persistent.Data.UseRelativeDataDir;
            }
            set
            {
                persistent.Data.UseRelativeDataDir = value;
                persistent.Save();
            }
        }

        public string SpecificBuildNumber
        {
            get
            {
                return persistent.Data.SpecificBuildNumber;
            }
            set
            {
                persistent.Data.SpecificBuildNumber = value;
                persistent.Save();
            }
        }
        public LaunchChoices LastLaunchChoice
        {
            get
            {
                return persistent.Data.LastLaunchChoice;
            }
            set
            {
                persistent.Data.LastLaunchChoice = value;
                persistent.Save();
            }
        }

        public bool UpdateSourceEstablished
        {
            get
            {
                return persistent.Data.UpdateSourceEstablished;
            }
            set
            {
                persistent.Data.UpdateSourceEstablished = value;
                persistent.Save();
            }
        }

        public string WurmAssistantWebServiceUrl
        {
            get
            {
                return persistent.Data.WurmAssistantWebServiceUrl;
            }
            set
            {
                persistent.Data.WurmAssistantWebServiceUrl = value;
                persistent.Save();
            }
        }
    }
}