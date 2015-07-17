using System;
using System.IO;

using AldurSoft.Core;

namespace AldurSoft.WurmApi.Impl.WurmCharactersImpl
{
    class PlayerConfigWatcher : IRequireRefresh, IDisposable
    {
        private readonly string playerDirectoryFullPath;
        private readonly FileSystemChangeMonitor fileMonitor;
        private readonly string configDefinerFullPath;

        private string currentConfigName;
        private const string ConfigDefinerFileName = "config.txt";

        public event EventHandler Changed;

        public string CurrentConfigName
        {
            get { return currentConfigName; }
        }

        protected virtual void OnChanged()
        {
            EventHandler handler = Changed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public PlayerConfigWatcher(string playerDirectoryFullPath)
        {
            if (playerDirectoryFullPath == null) throw new ArgumentNullException("playerDirectoryFullPath");
            this.playerDirectoryFullPath = playerDirectoryFullPath;

            configDefinerFullPath = Path.Combine(playerDirectoryFullPath, ConfigDefinerFileName);
            fileMonitor = new FileSystemChangeMonitor()
            {
                FullPath = playerDirectoryFullPath,
                Filter = ConfigDefinerFileName
            };
            fileMonitor.EnableRaisingEvents = true;
            RereadCurrentConfigName();
        }

        private void RereadCurrentConfigName()
        {
            var newConfigName = File.ReadAllText(configDefinerFullPath).Trim();
            if (CurrentConfigName != newConfigName)
            {
                currentConfigName = newConfigName;
                OnChanged();
            }
        }

        public void Refresh()
        {
            var changed = fileMonitor.GetAnyChangeAndReset();
            if (changed)
            {
                RereadCurrentConfigName();
            }
        }

        public void Dispose()
        {
            fileMonitor.Dispose();
        }
    }
}
