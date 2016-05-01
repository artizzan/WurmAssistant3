using Newtonsoft.Json.Serialization;

namespace AldursLab.PersistentObjects.Serialization
{
    public interface IJsonErrorHandlingStrategy
    {
        void HandleErrorOnDeserialize(object o, ErrorEventArgs args);
        void HandleErrorOnSerialize(object o, ErrorEventArgs args);
        PreviewResult PreviewJsonStringOnPopulate(string rawJson, object populatedObject);
    }
}