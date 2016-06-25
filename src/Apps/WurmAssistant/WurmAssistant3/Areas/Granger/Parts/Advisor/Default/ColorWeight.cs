namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.Advisor.Default
{
    public class ColorWeight
    {
        public readonly CreatureColor Color;
        public float Weight;

        public ColorWeight(CreatureColor color, float weight)
        {
            Color = color;
            Weight = weight;
        }
    }
}