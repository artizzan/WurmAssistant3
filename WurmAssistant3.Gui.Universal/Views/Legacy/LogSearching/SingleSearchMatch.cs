using System;

namespace AldursLab.WurmAssistant3.Gui.Universal.Views.Legacy.LogSearching
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