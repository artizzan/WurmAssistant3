namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    partial class ConflictResolveDialog
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
            this.buttonSkip = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonImportAsNew = new System.Windows.Forms.Button();
            this.labelText = new AldursLab.WurmAssistant3.Utils.WinForms.LabelAutowrap();
            this.checkBoxDoForAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSkip
            // 
            this.buttonSkip.Location = new System.Drawing.Point(6, 19);
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Size = new System.Drawing.Size(143, 41);
            this.buttonSkip.TabIndex = 0;
            this.buttonSkip.Text = "Skip";
            this.buttonSkip.UseVisualStyleBackColor = true;
            this.buttonSkip.Click += new System.EventHandler(this.buttonSkip_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Location = new System.Drawing.Point(155, 19);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(143, 41);
            this.buttonReplace.TabIndex = 1;
            this.buttonReplace.Text = "Replace with imported";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonImportAsNew
            // 
            this.buttonImportAsNew.Location = new System.Drawing.Point(304, 19);
            this.buttonImportAsNew.Name = "buttonImportAsNew";
            this.buttonImportAsNew.Size = new System.Drawing.Size(143, 41);
            this.buttonImportAsNew.TabIndex = 2;
            this.buttonImportAsNew.Text = "Import as new trigger";
            this.buttonImportAsNew.UseVisualStyleBackColor = true;
            this.buttonImportAsNew.Click += new System.EventHandler(this.buttonImportAsNew_Click);
            // 
            // labelText
            // 
            this.labelText.Location = new System.Drawing.Point(12, 9);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(566, 13);
            this.labelText.TabIndex = 4;
            this.labelText.Text = "Sample Info";
            // 
            // checkBoxDoForAll
            // 
            this.checkBoxDoForAll.AutoSize = true;
            this.checkBoxDoForAll.Location = new System.Drawing.Point(12, 66);
            this.checkBoxDoForAll.Name = "checkBoxDoForAll";
            this.checkBoxDoForAll.Size = new System.Drawing.Size(178, 17);
            this.checkBoxDoForAll.TabIndex = 3;
            this.checkBoxDoForAll.Text = "Do this for every imported trigger";
            this.checkBoxDoForAll.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.checkBoxDoForAll);
            this.groupBox1.Controls.Add(this.buttonSkip);
            this.groupBox1.Controls.Add(this.buttonImportAsNew);
            this.groupBox1.Controls.Add(this.buttonReplace);
            this.groupBox1.Location = new System.Drawing.Point(15, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 95);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // ConflictResolveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 205);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConflictResolveDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Resolve Import Conflict";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSkip;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonImportAsNew;
        private Utils.WinForms.LabelAutowrap labelText;
        private System.Windows.Forms.CheckBox checkBoxDoForAll;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}