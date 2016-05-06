using System;

namespace AldursLab.WurmAssistant3.Areas.LogSearcher.Views
{
    public class SingleSearchMatch
    {
        public long BeginCharPos { get; private set; }
        public long LenghtChars { get; private set; }
        public DateTime MatchDate { get; private set; }

        internal SingleSearchMatch(long beginCharPos, long endCharPos, DateTime matchDate)
        {
            this.BeginCharPos = beginCharPos;
            this.LenghtChars = endCharPos;
            this.MatchDate = matchDate;
        }
    }
}