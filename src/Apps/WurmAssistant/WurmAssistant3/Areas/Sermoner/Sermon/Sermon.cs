using System;

namespace AldursLab.WurmAssistant3.Areas.Sermoner.Sermon
{
    public class Sermon
    {
        public string Name;
        public DateTime SermonFinished;

        public Sermon(string name, DateTime sermonFinished)
        {
            Name = name;
            SermonFinished = sermonFinished;
        }
    }
}
