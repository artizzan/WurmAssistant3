using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AldursLab.WurmAssistant3.Areas.Sermoner.Sermon
{
    public class Preacher 
    {
        private string _name;
        public string Name 
        { 
            get { return _name; } 
            set
            {
                if (value != _name)
                {
                    _name = value;
                }
            }
        }
        private DateTime _lastSermon;
        /// <summary>
        /// Last (newest) sermon datetime
        /// </summary>
        public DateTime LastSermon
        {
            get { return _lastSermon; } 
            set 
            {
                if (_lastSermon != value)
                {
                    _lastSermon = value;
                }
            }
        }

        /// <summary>
        /// Minutes elapsed since last sermon
        /// </summary>
        public int ElapsedMinutes
        {
            get
            {
                return Convert.ToInt32(DateTime.Now.Subtract(_lastSermon).TotalMinutes);
            }
        }

        /// <summary>
        /// Time elapsed since last sermon
        /// </summary>
        public string Elapsed
        {
            get
            {
                return MinutesToTimeText(Convert.ToInt32(DateTime.Now.Subtract(_lastSermon).TotalMinutes));
            }
        }

        /// <summary>
        /// Minutes remaining cooldown since last sermon
        /// </summary>
        public string RemainingCooldown
        {
            get 
            {
                int cd = 180 - Convert.ToInt32(DateTime.Now.Subtract(_lastSermon).TotalMinutes);
                if (cd > 0)
                    return MinutesToTimeText(cd);
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// Number of sermons scanned
        /// </summary>
        public int SermonCount
        {
            get { return this.Sermons.Count; }
        }

        public List<Sermon> Sermons;
        
        public Preacher(string name) 
        {
            Sermons = new List<Sermon>();
            Name = name;
        }

        public int MinutesSinceLastPreach
        {
            get { return Convert.ToInt32(DateTime.Now.Subtract(LastSermon).TotalMinutes); }
        }

        public int PreachCount { 
            get { return Sermons.Count; }
        }

        public void AddSermon(DateTime sermonTime)
        {
            if (sermonTime > LastSermon)
            {
                LastSermon = sermonTime;
            }
            Sermons.Add(new Sermon(Name, sermonTime));
        }

        public int CooldownLeft()
        {
            int diff = Convert.ToInt32(DateTime.Now.Subtract(LastSermon).TotalMinutes);
            if (diff < 180)
            {
                return 180 - diff;
            }
            else
            {
                return 0;
            }
        }

        private string MinutesToTimeText(int minutes)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
            if (timeSpan.Days > 1)
            {
                sb.Append(String.Format("{0} days ", timeSpan.Days));
                sb.Append(timeSpan.ToString(@"hh\:mm\:ss"));
            }
            else if (timeSpan.Days > 0)
            {
                sb.Append(String.Format("{0} day ", timeSpan.Days));
                sb.Append(timeSpan.ToString(@"hh\:mm\:ss"));
            }
            else
            {
                sb.Append(timeSpan.ToString(@"hh\:mm\:ss"));
            }
            return sb.ToString();
        }
    }
}
