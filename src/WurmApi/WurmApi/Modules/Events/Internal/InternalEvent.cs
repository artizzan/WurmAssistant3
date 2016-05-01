namespace AldursLab.WurmApi.Modules.Events.Internal
{
    abstract class InternalEvent
    {
        /// <summary>
        /// Flags the event as pending. Will be invoked according to scheduling parameters.
        /// </summary>
        public abstract void Trigger();

        /// <summary>
        /// Detaches the event. Further signals will be ignored.
        /// Detached events cannot be reattached, create new.
        /// </summary>
        public abstract void Detach();
    }
}