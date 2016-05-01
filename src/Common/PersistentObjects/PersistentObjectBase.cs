using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AldursLab.PersistentObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class PersistentObjectBase : IPersistentObject
    {
        //note: [JsonExtensionData] member has been removed.
        //reason: Preservation of unknown json tokens does not work correctly in all cases and should be avoided.

        protected PersistentObjectBase([NotNull] string persistentObjectId = "")
        {
            if (persistentObjectId == null) throw new ArgumentNullException("persistentObjectId");
            PersistentObjectId = persistentObjectId;
        }

        public virtual void FlagAsChanged()
        {
            PersistibleChangesPending = true;
        }

        public string PersistentObjectId { get; private set; }

        public bool PersistibleChangesPending { get; set; }

        public event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        /// <summary>
        /// Requests PersistenceManager to immediatelly save this object state.
        /// </summary>
        /// <param name="forceSave">Default true. If set to false, save will only happen if PersistibleChangesPending is true.</param>
        public virtual void RequestSave(bool forceSave = true)
        {
            var handler = PersistentStateSaveRequested;
            if (handler != null) handler(this, new SaveEventArgs(forceSave));
        }

        void IPersistentObject.PersistentDataLoaded()
        {
            OnPersistentDataLoaded();
        }

        protected virtual void OnPersistentDataLoaded(){}
    }
}
