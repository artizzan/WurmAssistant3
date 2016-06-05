using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Areas.TrayPopups.Parts
{
    public partial class FormPopupContainer : Form
    {
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        struct PopupQueueItem
        {
            public string Title;
            public string Content;
            public int TimeToShowMillis;
            public PopupQueueItem(string title, string content, int timeToShowMillis)
            {
                if (title != null) Title = title; else Title = "Wurm Assistant";
                if (content != null) Content = content; else Content = "";
                TimeToShowMillis = timeToShowMillis.ConstrainToRange(1000, int.MaxValue);
            }
        }

        Queue<PopupQueueItem> PopupQueue = new Queue<PopupQueueItem>();
        int PopupQueueDelay = 0;
        string DefaultTitle = "";

        public FormPopupContainer()
        {
            InitializeComponent();
            timer1.Enabled = true;
        }

        private void popupNotifier1_Click(object sender, EventArgs e)
        {
            popupNotifier1.Hide();
            PopupQueueDelay = 250;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HandlePopupQueue();
        }

        void HandlePopupQueue()
        {
            if (PopupQueue.Count != 0 && PopupQueueDelay <= 0)
            {
                PopupQueueItem item = PopupQueue.Dequeue();
                popupNotifier1.TitleText = item.Title;
                popupNotifier1.ContentText = item.Content;
                AppenedMoreMessagesWithSameTitle(item, 3);
                popupNotifier1.Delay = item.TimeToShowMillis;
                popupNotifier1.Popup();
                PopupQueueDelay = item.TimeToShowMillis + 250;
            }
            else PopupQueueDelay -= timer1.Interval;
        }

        void AppenedMoreMessagesWithSameTitle(PopupQueueItem item, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                if (!TryAppendMessage(item)) break;
            }
        }

        bool TryAppendMessage(PopupQueueItem item)
        {
            if (PopupQueue.Count > 0 && PopupQueue.Peek().Title == item.Title)
            {
                popupNotifier1.ContentText += "\n" + PopupQueue.Dequeue().Content;
                return true;
            }
            return false;
        }

        public void ScheduleCustomPopupNotify(string title, string content, int timeToShowMillis = 4000)
        {
            PopupQueue.Enqueue(
                new PopupQueueItem(
                    String.IsNullOrEmpty(title) ? DefaultTitle : title, 
                    content, 
                    timeToShowMillis));
        }

        public void CloseThisContainer()
        {
            Application.ExitThread();
        }

        private void FormPopupContainer_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        internal void SetDefaultTitle(string title)
        {
            this.DefaultTitle = title;
        }

        private void popupNotifier1_Close(object sender, EventArgs e)
        {
            popupNotifier1.Hide();
            PopupQueueDelay = 250;
        }
    }
}
