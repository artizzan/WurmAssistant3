using System;

namespace AldursLab.PersistentObjects
{
    public interface IPersistentObject
    {
        /// <summary>
        /// Unique id, which identifies this object in persistent store.
        /// This property should not be null and should not change during object lifetime. 
        /// Changes will be ignored. Ensure it is set before wiring.
        /// </summary>
        string PersistentObjectId { get; }

        /// <summary>
        /// Set to true to indicate, that saving this object is required. 
        /// After saving, this property will always revert to false.
        /// </summary>
        bool PersistibleChangesPending { get; set; }

        /// <summary>
        /// Trigger this event to request immediate save of the object.
        /// SaveEventArgs parameters control saving enforcement.
        /// </summary>
        event EventHandler<SaveEventArgs> PersistentStateSaveRequested;

        /// <summary>
        /// Called once during object lifetime, just after all persistent data of the object is loaded.
        /// </summary>
        void PersistentDataLoaded();
    }
}