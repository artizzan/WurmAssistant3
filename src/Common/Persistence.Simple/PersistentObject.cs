using System;
using System.ComponentModel;
using System.Threading;
using Notify;

namespace AldursLab.Persistence.Simple
{
    internal class PersistentObject
    {
        readonly Notify.Tracker tracker;
        int changedAndUnsaved;

        public string Id { get; }
        public object Obj { get; }

        public PersistentObject(string id, object obj)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            this.Id = id;
            this.Obj = obj;

            var npc = obj as INotifyPropertyChanged;
            if (npc == null)
            {
                throw new InvalidOperationException($"{nameof(obj)} must implement {nameof(INotifyPropertyChanged)}");
            }

            tracker = new Tracker();
            tracker.Track(obj);
            tracker.Changed += _ => ChangedAndUnsaved = true;
        }

        public bool ChangedAndUnsaved
        {
            get { return changedAndUnsaved == 1; }
            private set { changedAndUnsaved = value == true ? 1 : 0; }
        }

        public void StopTrackingChanges()
        {
            changedAndUnsaved = 0;
            tracker?.Dispose();
        }

        public void SaveAndFlagUnchanged(ISerializer serializer, IDataStorage dataStorage, string rootPath)
        {
            var wasUnsaved = Interlocked.Exchange(ref changedAndUnsaved, 1);
            ChangedAndUnsaved = false;

            try
            {
                var serialized = serializer.Serialize(Obj);
                dataStorage.Save(rootPath, Id, serialized);
            }
            catch(Exception)
            {
                if (wasUnsaved == 1)
                {
                    ChangedAndUnsaved = true;
                }
                throw;
            }
        }
    }
}