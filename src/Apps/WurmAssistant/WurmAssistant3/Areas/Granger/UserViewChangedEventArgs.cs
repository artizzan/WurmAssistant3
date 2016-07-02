using System;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class UserViewChangedEventArgs : EventArgs
    {
        public readonly bool HerdViewVisible;
        public readonly bool TraitViewVisible;
        public UserViewChangedEventArgs(bool herdViewVisible, bool traitViewVisible)
        {
            this.HerdViewVisible = herdViewVisible;
            this.TraitViewVisible = traitViewVisible;
        }
    }
}