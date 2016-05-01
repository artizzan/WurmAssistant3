using System;
using System.IO;
using AldursLab.Essentials.FileSystem;

namespace AldursLab.Persistence
{
    public class Persistent<T> where T:class, new()
    {
        private readonly string filePath;
        public T Data { get; private set; }

        private readonly DataSerializer serializer = new JsonTypelessSerializer();

        public Persistent(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (!Path.IsPathRooted(filePath))
            {
                throw new ArgumentException("filePath must be rooted (absolute).");
            }
            this.filePath = filePath;
            Data = new T();
        }

        public void Load()
        {
            var serialized = ReliableFileOps.TryReadFileContents(filePath);
            if (serialized != null)
            {
                Data = serializer.Deserialize<T>(serialized);
            }
        }

        public void Save()
        {
            var serialized = serializer.Serialize(Data);
            ReliableFileOps.SaveFileContents(filePath, serialized);
        }
    }
}
