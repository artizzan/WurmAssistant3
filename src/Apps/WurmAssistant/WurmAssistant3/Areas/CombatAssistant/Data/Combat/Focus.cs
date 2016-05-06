namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat
{
    public class Focus
    {
        public FocusLevel FocusLevel { get; set; }

        public void LowerByOneLevel()
        {
            var newLvl = (int) FocusLevel - 1;
            if (newLvl < 0) newLvl = 0;
            FocusLevel = (FocusLevel)newLvl;
        }

        public override string ToString()
        {
            return FocusLevel.ToString();
        }
    }

    public enum FocusLevel
    {
        NotFocused = 0,
        Balanced = 1,
        Focused = 2,
        Lighting = 3,
        Lifted = 4,
        Supernatural = 5
    }
}