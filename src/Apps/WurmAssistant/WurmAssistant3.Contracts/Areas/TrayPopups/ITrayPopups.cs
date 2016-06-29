namespace AldursLab.WurmAssistant3.Areas.TrayPopups
{
    public interface  ITrayPopups
    {
        /// <summary>
        /// Add message to Popup queue
        /// </summary>
        /// <param name="title">title of the message</param>
        /// <param name="content">content of the message</param>
        /// <param name="timeToShowMillis">how long should this popup be visible, in milliseconds</param>
        void Schedule(string content, string title, int timeToShowMillis = 3000);

        /// <summary>
        /// Add message to Popup queue with default title
        /// </summary>
        /// <param name="content">content of the message</param>
        /// <param name="timeToShowMillis">how long should this popup be visible, in milliseconds</param>
        void Schedule(string content, int timeToShowMillis = 3000);
    }
}
