using System;

namespace AldursLab.WurmAssistant.Launcher.Contracts
{
    [Flags]
    public enum LaunchChoices
    {
        None = 0,
        Dev = 1, 
        Beta = 2,
        StableWin = 4,
        StableMac = 8,
        StableLin = 16,
        WurmUnlimited = 32
    }
}