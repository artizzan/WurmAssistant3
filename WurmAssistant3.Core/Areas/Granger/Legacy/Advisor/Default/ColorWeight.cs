namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.Advisor.Default
{
    public class ColorWeight
    {
        public readonly HorseColor Color;
        public float Weight;

        public ColorWeight(HorseColor color, float weight)
        {
            Color = color;
            Weight = weight;
        }
    }
}