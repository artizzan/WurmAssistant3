namespace AldursLab.WurmAssistant3.Areas.Config.Views
{
    partial class FirstTimeSetupView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.wurmOnlineClientDirPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOsMac = new System.Windows.Forms.RadioButton();
            this.rbOsLinux = new System.Windows.Forms.RadioButton();
            this.rbOsWindows = new System.Windows.Forms.RadioButton();
            this.labelPathDescription = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFindWurmDir = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(585, 267);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(693, 267);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // wurmOnlineClientDirPath
            // 
            this.wurmOnlineClientDirPath.Location = new System.Drawing.Point(122, 212);
            this.wurmOnlineClientDirPath.Margin = new System.Windows.Forms.Padding(4);
            this.wurmOnlineClientDirPath.Name = "wurmOnlineClientDirPath";
            this.wurmOnlineClientDirPath.Size = new System.Drawing.Size(671, 23);
            this.wurmOnlineClientDirPath.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOsMac);
            this.groupBox1.Controls.Add(this.rbOsLinux);
            this.groupBox1.Controls.Add(this.rbOsWindows);
            this.groupBox1.Location = new System.Drawing.Point(15, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(267, 123);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Choose you current operating system";
            // 
            // rbOsMac
            // 
            this.rbOsMac.AutoSize = true;
            this.rbOsMac.Enabled = false;
            this.rbOsMac.Location = new System.Drawing.Point(9, 81);
            this.rbOsMac.Margin = new System.Windows.Forms.Padding(4);
            this.rbOsMac.Name = "rbOsMac";
            this.rbOsMac.Size = new System.Drawing.Size(190, 21);
            this.rbOsMac.TabIndex = 2;
            this.rbOsMac.TabStop = true;
            this.rbOsMac.Text = "Mac OS X - not supported";
            this.rbOsMac.UseVisualStyleBackColor = true;
            // 
            // rbOsLinux
            // 
            this.rbOsLinux.AutoSize = true;
            this.rbOsLinux.Enabled = false;
            this.rbOsLinux.Location = new System.Drawing.Point(9, 53);
            this.rbOsLinux.Margin = new System.Windows.Forms.Padding(4);
            this.rbOsLinux.Name = "rbOsLinux";
            this.rbOsLinux.Size = new System.Drawing.Size(244, 21);
            this.rbOsLinux.TabIndex = 1;
            this.rbOsLinux.TabStop = true;
            this.rbOsLinux.Text = "Linux (eg. Ubuntu) - not supported";
            this.rbOsLinux.UseVisualStyleBackColor = true;
            // 
            // rbOsWindows
            // 
            this.rbOsWindows.AutoSize = true;
            this.rbOsWindows.Location = new System.Drawing.Point(9, 25);
            this.rbOsWindows.Margin = new System.Windows.Forms.Padding(4);
            this.rbOsWindows.Name = "rbOsWindows";
            this.rbOsWindows.Size = new System.Drawing.Size(82, 21);
            this.rbOsWindows.TabIndex = 0;
            this.rbOsWindows.TabStop = true;
            this.rbOsWindows.Text = "Windows";
            this.rbOsWindows.UseVisualStyleBackColor = true;
            // 
            // labelPathDescription
            // 
            this.labelPathDescription.AutoSize = true;
            this.labelPathDescription.Location = new System.Drawing.Point(12, 155);
            this.labelPathDescription.Name = "labelPathDescription";
            this.labelPathDescription.Size = new System.Drawing.Size(156, 51);
            this.labelPathDescription.TabIndex = 4;
            this.labelPathDescription.Text = "This label is set in code\r\nline2\r\nline3";
            // 
            // btnFindWurmDir
            // 
            this.btnFindWurmDir.Location = new System.Drawing.Point(15, 209);
            this.btnFindWurmDir.Name = "btnFindWurmDir";
            this.btnFindWurmDir.Size = new System.Drawing.Size(100, 29);
            this.btnFindWurmDir.TabIndex = 5;
            this.btnFindWurmDir.Text = "Find";
            this.btnFindWurmDir.UseVisualStyleBackColor = true;
            this.btnFindWurmDir.Click += new System.EventHandler(this.btnFindWurmDir_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(289, 17);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(504, 119);
            this.textBox1.TabIndex = 6;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "Support for non-windows has been disabled and will not be back. \r\nWurm Assistant " +
    "is very sorry!\r\n\r\nIf needed, this setup can be run again from Options menu.";
            // 
            // FirstTimeSetupView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 308);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnFindWurmDir);
            this.Controls.Add(this.labelPathDescription);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.wurmOnlineClientDirPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstTimeSetupView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Window title is set in code";
            this.Load += new System.EventHandler(this.FirstTimeSetupView_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox wurmOnlineClientDirPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOsMac;
        private System.Windows.Forms.RadioButton rbOsLinux;
        private System.Windows.Forms.RadioButton rbOsWindows;
        private System.Windows.Forms.Label labelPathDescription;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFindWurmDir;
        private System.Windows.Forms.TextBox textBox1;
    }
}