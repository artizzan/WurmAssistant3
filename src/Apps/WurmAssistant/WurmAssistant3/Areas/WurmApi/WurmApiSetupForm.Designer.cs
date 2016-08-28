namespace AldursLab.WurmAssistant3.Areas.WurmApi
{
    partial class WurmApiSetupForm
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
            this.labelPathDescription = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFindWurmDir = new System.Windows.Forms.Button();
            this.autodetectFailedLabel2 = new AldursLab.WurmAssistant3.Utils.WinForms.LabelAutowrap();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(585, 121);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(693, 121);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // wurmOnlineClientDirPath
            // 
            this.wurmOnlineClientDirPath.Location = new System.Drawing.Point(122, 66);
            this.wurmOnlineClientDirPath.Margin = new System.Windows.Forms.Padding(4);
            this.wurmOnlineClientDirPath.Name = "wurmOnlineClientDirPath";
            this.wurmOnlineClientDirPath.Size = new System.Drawing.Size(671, 23);
            this.wurmOnlineClientDirPath.TabIndex = 2;
            // 
            // labelPathDescription
            // 
            this.labelPathDescription.AutoSize = true;
            this.labelPathDescription.Location = new System.Drawing.Point(12, 9);
            this.labelPathDescription.Name = "labelPathDescription";
            this.labelPathDescription.Size = new System.Drawing.Size(156, 51);
            this.labelPathDescription.TabIndex = 4;
            this.labelPathDescription.Text = "This label is set in code\r\nline2\r\nline3";
            // 
            // btnFindWurmDir
            // 
            this.btnFindWurmDir.Location = new System.Drawing.Point(15, 63);
            this.btnFindWurmDir.Name = "btnFindWurmDir";
            this.btnFindWurmDir.Size = new System.Drawing.Size(100, 29);
            this.btnFindWurmDir.TabIndex = 5;
            this.btnFindWurmDir.Text = "Find";
            this.btnFindWurmDir.UseVisualStyleBackColor = true;
            this.btnFindWurmDir.Click += new System.EventHandler(this.btnFindWurmDir_Click);
            // 
            // autodetectFailedLabel2
            // 
            this.autodetectFailedLabel2.Location = new System.Drawing.Point(12, 95);
            this.autodetectFailedLabel2.Name = "autodetectFailedLabel2";
            this.autodetectFailedLabel2.Size = new System.Drawing.Size(563, 17);
            this.autodetectFailedLabel2.TabIndex = 7;
            this.autodetectFailedLabel2.Text = "autodetect error text set in code";
            this.autodetectFailedLabel2.Visible = false;
            // 
            // WurmApiSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 167);
            this.Controls.Add(this.autodetectFailedLabel2);
            this.Controls.Add(this.btnFindWurmDir);
            this.Controls.Add(this.labelPathDescription);
            this.Controls.Add(this.wurmOnlineClientDirPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WurmApiSetupForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Window title is set in code";
            this.Load += new System.EventHandler(this.FirstTimeSetupView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox wurmOnlineClientDirPath;
        private System.Windows.Forms.Label labelPathDescription;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFindWurmDir;
        private Utils.WinForms.LabelAutowrap autodetectFailedLabel2;
    }
}