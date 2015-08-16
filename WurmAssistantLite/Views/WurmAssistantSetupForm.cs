using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistantLite.Views
{
    public partial class WurmAssistantSetupForm : Form
    {
        public WurmAssistantSetupForm()
        {
            InitializeComponent();
        }

        public string WurmOnlineClientFullPath
        {
            get { return wurmOnlineClientDirPath.Text; }
            set { wurmOnlineClientDirPath.Text = value; }
        }

        public Platform OperatingSystem
        {
            get
            {
                Platform result = Platform.Unknown;
                if (rbOsLinux.Checked) result = Platform.Linux;
                if (rbOsMac.Checked) result = Platform.Mac;
                if (rbOsWindows.Checked) result = Platform.Windows;
                return result;
            }
            set
            {
                if (value == Platform.Linux)
                    rbOsLinux.Checked = true;
                else if (value == Platform.Mac)
                    rbOsMac.Checked = true;
                else if (value == Platform.Windows)
                    rbOsWindows.Checked = true;
                else if (value == Platform.Unknown)
                {
                    rbOsLinux.Checked = rbOsMac.Checked = rbOsWindows.Checked = false;
                }
                else
                {
                    throw new InvalidOperationException("Unknow Platform value: " + value);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var platform = OperatingSystem;
            if (platform == Platform.Unknown)
            {
                MessageBox.Show("Operating system must be selected");
                return;
            }

            var rooted = Path.IsPathRooted(wurmOnlineClientDirPath.Text);
            if (!rooted)
            {
                MessageBox.Show(@"Path must be absolute, eg. C:\games\wurm or /home/MyUbuntu/wurm");
                return;
            }
            var exists = Directory.Exists(wurmOnlineClientDirPath.Text);
            if (!exists)
            {
                MessageBox.Show("Directory does not exist");
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnFindWurmDir_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                wurmOnlineClientDirPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
