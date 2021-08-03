namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class TraitItem
    {
        public TraitDisplayMode DisplayMode = TraitDisplayMode.Full;

        public CreatureTrait Trait;
        public bool Exists;
        public int Value;

        public string TraitAspect
        {
            get
            {
                if (DisplayMode == TraitDisplayMode.Compact)
                {
                    return Trait.ToCompactString();
                }
                else if (DisplayMode == TraitDisplayMode.Shortcut)
                {
                    return Trait.ToShortcutString();
                }
                else
                {
                    return Trait.ToString();
                }
            }
        }
        public string HasAspect
        {
            get
            {
                if (Exists)
                {
                    return "YES";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string ValueAspect => Value.ToString();

        public System.Drawing.Color? BackColor
        {
            get
            {
                if (DisableBackgroundColors)
                {
                    return null;
                }

                if (Exists)
                {
                    if (Value > 0)
                    {
                        return System.Drawing.Color.LightGreen;
                    }
                    else if (Value < 0)
                    {
                        return System.Drawing.Color.OrangeRed;
                    }
                }
                return null;
            }
        }

        public bool DisableBackgroundColors { get; set; }
    }
}