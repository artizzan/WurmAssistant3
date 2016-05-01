using System;

namespace WurmAssistantDataTransfer.Dtos
{
    public class Sound
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }
        public byte[] FileData { get; set; }
        public string FileNameWithExt { get; set; }
    }
}