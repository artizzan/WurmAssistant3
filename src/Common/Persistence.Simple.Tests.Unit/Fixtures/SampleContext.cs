namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    public class SampleContext : PersistentContext
    {
        readonly IObjectSet<SampleObject> typedSet;
        readonly IObjectSet untypedSet;
        readonly SampleObject o1;
        readonly SampleObject o2;
        readonly SampleObjectWithNotifyError ononotify;

        public SampleContext(ISerializer serializer, IDataStorage dataStorage) : base(serializer, dataStorage)
        {
            typedSet = GetOrCreateObjectSet<SampleObject>("SampleObjects");
            o1 = typedSet.GetOrCreate("oid1");
            o2 = typedSet.GetOrCreate("oid2");

            untypedSet = GetOrCreateObjectSet("untyped");
            ononotify = untypedSet.GetOrCreate<SampleObjectWithNotifyError>("ononotify");
        }


        public IObjectSet<SampleObject> TypedSet => typedSet;

        public SampleObject O1 => o1;

        public SampleObject O2 => o2;

        public IObjectSet UntypedSet => untypedSet;

        public SampleObjectWithNotifyError ONoNotify => ononotify;
    }
}