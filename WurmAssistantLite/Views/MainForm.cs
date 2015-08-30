using System;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistantLite.Bootstrapping;

namespace AldursLab.WurmAssistantLite.Views
{
    public partial class MainForm : Form
    {
        AppBootstrapper bootstrapper;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            initTimer.Enabled = true;
        }

        public void SetAppCoreView(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            mainAppPanel.Controls.Clear();
            mainAppPanel.Controls.Add(userControl);
        }

        public void SetLogOutputView(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            mainAppPanel.Controls.Clear();
            logOutputPanel.Controls.Add(userControl);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            if (bootstrapper != null) bootstrapper.Dispose();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            bootstrapper.Update();
        }

        private void initTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                bootstrapper = new AppBootstrapper(this);
                bootstrapper.Bootstrap();
                timer.Start();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.ToString(),
                    "WurmAssistant initialization error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
            finally
            {
                initTimer.Enabled = false;
            }
        }
    }
}
