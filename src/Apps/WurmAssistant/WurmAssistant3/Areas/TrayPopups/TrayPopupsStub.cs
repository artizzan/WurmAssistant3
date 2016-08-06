namespace AldursLab.WurmAssistant3.Areas.TrayPopups
{
    // Do not bind to kernel, this is for stubbing purposes only.
    class TrayPopupsStub : ITrayPopups
    {
        public void Schedule(string content, string title, int timeToShowMillis = 3000)
        {
        }

        public void Schedule(string content, int timeToShowMillis = 3000)
        {
        }
    }
}