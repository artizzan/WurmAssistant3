namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath
{
    partial class MeditPathTimerOptionsForm
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
            this.buttonManualCD = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonManualCD
            // 
            this.buttonManualCD.Location = new System.Drawing.Point(12, 12);
            this.buttonManualCD.Name = "buttonManualCD";
            this.buttonManualCD.Size = new System.Drawing.Size(258, 61);
            this.buttonManualCD.TabIndex = 0;
            this.buttonManualCD.Text = "Set cooldown manually";
            this.buttonManualCD.UseVisualStyleBackColor = true;
            this.buttonManualCD.Click += new System.EventHandler(this.buttonManualCD_Click);
            // 
            // MeditPathTimerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 85);
            this.Controls.Add(this.buttonManualCD);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeditPathTimerSettings";
            this.ShowIcon = false;
            this.Text = "Medit question timer options";
            this.Load += new System.EventHandler(this.MeditPathTimerOptions_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonManualCD;
    }
}