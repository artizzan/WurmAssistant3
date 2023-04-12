using AldursLab.WurmApi;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Insights;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using Ninject.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AldursLab.WurmAssistant3.Areas.Sermoner.Sermon
{
    public class PreacherList : List<Preacher>
    {
        protected IWurmCharacter WurmCharacter;
        protected ServerGroup playerTimersGroup;

        public event EventHandler AsyncInitCompleted;

        public PreacherList(IWurmCharacter character)
        {
            WurmCharacter = character;
        }

        public void AddSermon(string name, DateTime sermonTime)
        {
            Preacher preacher = this.Find(x => x.Name.Equals(name));

            if (preacher != null)
            {
                preacher.AddSermon(sermonTime);
            }
            else
            {
                preacher = new Preacher(name);
                preacher.AddSermon(sermonTime);
                this.Add(preacher);
            }
        }

        public async void PerformAsyncInit()
        {
            try
            {
                var results = await WurmCharacter.Logs.ScanLogsAsync(DateTime.Now - TimeSpan.FromDays(30), DateTime.Now, LogType.Event);
                var lines = results.ToList();

                foreach (LogEntry line in lines)
                {
                    HandleLogEntry(line);
                }
                AsyncInitCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                
            }
        }

        public void HandleLogEntry(LogEntry line)
        {
            if (line.Content.Contains("finish") && line.Content.Contains("sermon"))
            {
                string[] lineSplit = line.Content.Split(' ');
                string name = lineSplit[0];

                if (name.Equals("You"))
                    name = WurmCharacter.Name.ToString();

                AddSermon(name, line.Timestamp);
            }
        }

        public bool IsOnCooldown
        {
            get
            {
                if (this.Count > 0)
                {
                    DateTime last = LastSermon();
                    int diff = Convert.ToInt32(DateTime.Now.Subtract(last).TotalMinutes);
                    if (diff < 30)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        public DateTime LastSermon()
        {
            return this.Max(i => i.LastSermon);
        }

        public int CoolDownLeft
        {
            get
            {
                if (this.Count > 0)
                {
                    DateTime last = LastSermon();
                    int diff = Convert.ToInt32(DateTime.Now.Subtract(last).TotalMinutes);
                    if (diff < 30)
                    {
                        return 30 - diff;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
        }

        public void ClearOldSermons(int minutes)
        {
            this.RemoveAll(x => x.ElapsedMinutes > minutes);
        }
    }
}
