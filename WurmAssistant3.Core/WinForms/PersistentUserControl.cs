using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AldursLab.WurmAssistant3.Core.WinForms
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PersistentUserControl : ExtendedUserControl, IPersistentObject
    {
        [JsonExtensionData]
        protected IDictionary<string, JToken> JsonExtensionData;

        public PersistentUserControl()
            : this("")
        {
            // constructor for designer
        }

        protected PersistentUserControl([NotNull] string persistentObjectId)
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