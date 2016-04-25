using System;
using System.Diagnostics;
using System.IO;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.SoundManager.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.SoundManager.Modules
{
    [JsonObject(MemberSerialization.Fields)]
    class SoundResource : ISoundResource
    {
        string fileFullName;
        string name;
        float adjustedVolume;
        Guid id;

        public SoundResource(Guid? id = null)
        {
            AdjustedVolume = 0.5f;
            FileFullName = string.Empty;
            this.id = id ?? Guid.NewGuid();
        }

        public string FileFullName
        {
            get { return fileFullName; }
            set
            {
                Debug.Assert(value != null);
                fileFullName = value ?? string.Empty;
            }
        }

        public bool IsNull { get { return false; } }

        public float AdjustedVolume
        {
            get { return adjustedVolume; }
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= 1);
                adjustedVolume = value.ConstrainToRange(0,1);
            }
        }

        public string SoundId { get { return Path.GetFileNameWithoutExtension(fileFullName); } }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Debug.Assert(value != null);
                name = value;
            }
        }

        public Guid Id { get { return id; }}

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}