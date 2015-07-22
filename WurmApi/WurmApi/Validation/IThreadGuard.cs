namespace AldurSoft.WurmApi.Validation
{
    public interface IThreadGuard
    {
        /// <summary>
        /// Ensure, that current thread is the same, as the 
        /// thread that created this ThreadGuard instance.
        /// </summary>
        void ValidateCurrentThread();
    }
}
