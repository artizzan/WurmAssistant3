using System;
using System.Windows.Forms;
using Caliburn.Micro;

namespace AldursLab.WurmAssistantLite
{
    public partial class Form1 : Form
    {
        SampleViewModel vm;
        IEventAggregator eventAggregator;

        public Form1()
        {
            InitializeComponent();
            eventAggregator = new EventAggregator();
            vm = new SampleViewModel(this);
            eventAggregator.Subscribe(vm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            eventAggregator.PublishOnCurrentThread("test");
        }
    }
}
