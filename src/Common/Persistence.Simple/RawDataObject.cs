namespace AldursLab.Persistence.Simple
{
    public class RawDataObject
    {
        public string SetId { get; }
        public string ObjectId { get; }
        public string Data { get; }

        public RawDataObject(string setId, string objectId, string data)
        {
            SetId = setId;
            ObjectId = objectId;
            Data = data;
        }
    }
}