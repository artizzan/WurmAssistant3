namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public sealed class AreaScope
    {
        public string Name { get; }

        public AreaScope(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}