namespace AldursLab.WurmAssistant3.Core.Areas.RevealCreatures.Data
{
    public class FindResult
    {
        public string Creature { get; set; }
        public Distance Distance { get; set; }
        public Direction Direction { get; set; }
        public bool Highlighted { get; set; }
    }
}
