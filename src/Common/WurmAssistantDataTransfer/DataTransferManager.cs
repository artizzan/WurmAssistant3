using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistantDataTransfer
{
    public class DataTransferManager : IDataTransferManager
    {
        readonly JsonSerializer serializer;

        public DataTransferManager()
        {
            serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            });
        }

        public void SaveToFile(string filePath, WurmAssistantDto dto)
        {
            if (File.Exists(filePath))
                throw new ApplicationException(string.Format("File at filepath {0} already exists", filePath));

            var serialized = Serialize(dto);
            File.WriteAllText(filePath, serialized, Encoding.UTF8);
        }

        public WurmAssistantDto LoadFromFile(string filePath)
        {
            var serialized = File.ReadAllText(filePath, Encoding.UTF8);
            return Deserialize<WurmAssistantDto>(serialized);
        }

        string Serialize<T>(T @object)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jtw, @object);
                }
            }
            return sb.ToString();
        }

        T Deserialize<T>(string data)
        {
            using (var sr = new StringReader(data))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<T>(jtr);
                }
            }
        }
    }
}
