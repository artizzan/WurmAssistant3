namespace AldursLab.PersistentObjects
{
    public class SaveEventArgs
    {
        /// <summary>
        /// Should object be saved, even if no pending changes are indicated?
        /// </summary>
        public bool Force { get; private set; }

        /// <param name="force">Should object be saved, even if no pending changes are indicated?</param>
        public SaveEventArgs(bool force = false)
        {
            Force = force;
        }
    }
}