namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views
{
    partial class Wa2DataImportView
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
            this.labelAutowrap1 = new AldursLab.WurmAssistant3.Core.WinForms.LabelAutowrap();
            this.selectFileBtn = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAutowrap1.Location = new System.Drawing.Point(12, 9);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(364, 65);
            this.labelAutowrap1.TabIndex = 0;
            this.labelAutowrap1.Text = "Select file to import data from.\r\nYou can generate file in Wurm Assistant 2, opti" +
    "ons -> export data.\r\n\r\nA serie of screens will guide you through importing sound" +
    "s, triggers, timers and creatures.";
            // 
            // selectFileBtn
            // 
            this.selectFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectFileBtn.Location = new System.Drawing.Point(12, 97);
            this.selectFileBtn.Name = "selectFileBtn";
            this.selectFileBtn.Size = new System.Drawing.Size(262, 31);
            this.selectFileBtn.TabIndex = 1;
            this.selectFileBtn.Text = "Begin by selecting exported JSON file...\r\n";
            this.selectFileBtn.UseVisualStyleBackColor = true;
            this.selectFileBtn.Click += new System.EventHandler(this.selectFileBtn_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "JSON text file|*.json";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // Wa2DataImportView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 140);
            this.Controls.Add(this.selectFileBtn);
            this.Controls.Add(this.labelAutowrap1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wa2DataImportView";
            this.Text = "Data importer";
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.Button selectFileBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}