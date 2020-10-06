namespace AldursLab.WurmAssistant3.Areas.MainMenu
{
    partial class OptionsForm
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
            this.checkBoxGatherInsights = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.checkBoxAlternativePopupsStrategy = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxGatherInsights
            // 
            this.checkBoxGatherInsights.AutoSize = true;
            this.checkBoxGatherInsights.Location = new System.Drawing.Point(12, 12);
            this.checkBoxGatherInsights.Name = "checkBoxGatherInsights";
            this.checkBoxGatherInsights.Size = new System.Drawing.Size(312, 43);
            this.checkBoxGatherInsights.TabIndex = 0;
            this.checkBoxGatherInsights.Text = "Allow Wurm Assistant to send anonymous simple insights \r\nabout how the app and it" +
    "\'s features are used. \r\nData is used only for fixing issues and improving the fe" +
    "atures.";
            this.checkBoxGatherInsights.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(297, 147);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // checkBoxAlternativePopupsStrategy
            // 
            this.checkBoxAlternativePopupsStrategy.AutoSize = true;
            this.checkBoxAlternativePopupsStrategy.Location = new System.Drawing.Point(12, 70);
            this.checkBoxAlternativePopupsStrategy.Name = "checkBoxAlternativePopupsStrategy";
            this.checkBoxAlternativePopupsStrategy.Size = new System.Drawing.Size(320, 30);
            this.checkBoxAlternativePopupsStrategy.TabIndex = 2;
            this.checkBoxAlternativePopupsStrategy.Text = "Show notification popups in the top right corner of main screen\r\n(default: bottom" +
    " right)";
            this.checkBoxAlternativePopupsStrategy.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 182);
            this.Controls.Add(this.checkBoxAlternativePopupsStrategy);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.checkBoxGatherInsights);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.Text = "Wurm Assistant Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxGatherInsights;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkBoxAlternativePopupsStrategy;
    }
}