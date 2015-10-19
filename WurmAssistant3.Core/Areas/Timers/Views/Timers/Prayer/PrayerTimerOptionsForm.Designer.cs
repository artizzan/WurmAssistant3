namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.Prayer
{
    partial class PrayerTimerOptionsForm
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
            this.numericUpDownFavorWhenThis = new System.Windows.Forms.NumericUpDown();
            this.checkBoxFavorWhenMAX = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxNotifySound = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonChangeSound = new System.Windows.Forms.Button();
            this.textBoxSoundName = new System.Windows.Forms.TextBox();
            this.checkBoxNotifyPopup = new System.Windows.Forms.CheckBox();
            this.checkBoxPopupPersist = new System.Windows.Forms.CheckBox();
            this.checkBoxShowFaithSkill = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFavorWhenThis)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownFavorWhenThis
            // 
            this.numericUpDownFavorWhenThis.DecimalPlaces = 2;
            this.numericUpDownFavorWhenThis.Location = new System.Drawing.Point(10, 62);
            this.numericUpDownFavorWhenThis.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownFavorWhenThis.Name = "numericUpDownFavorWhenThis";
            this.numericUpDownFavorWhenThis.Size = new System.Drawing.Size(68, 20);
            this.numericUpDownFavorWhenThis.TabIndex = 0;
            this.numericUpDownFavorWhenThis.ValueChanged += new System.EventHandler(this.numericUpDownFavorWhenThis_ValueChanged);
            // 
            // checkBoxFavorWhenMAX
            // 
            this.checkBoxFavorWhenMAX.AutoSize = true;
            this.checkBoxFavorWhenMAX.Location = new System.Drawing.Point(103, 63);
            this.checkBoxFavorWhenMAX.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxFavorWhenMAX.Name = "checkBoxFavorWhenMAX";
            this.checkBoxFavorWhenMAX.Size = new System.Drawing.Size(128, 17);
            this.checkBoxFavorWhenMAX.TabIndex = 1;
            this.checkBoxFavorWhenMAX.Text = "my current MAX favor";
            this.checkBoxFavorWhenMAX.UseVisualStyleBackColor = true;
            this.checkBoxFavorWhenMAX.CheckedChanged += new System.EventHandler(this.checkBoxFavorWhenMAX_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 64);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "or";
            // 
            // checkBoxNotifySound
            // 
            this.checkBoxNotifySound.AutoSize = true;
            this.checkBoxNotifySound.Location = new System.Drawing.Point(10, 85);
            this.checkBoxNotifySound.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxNotifySound.Name = "checkBoxNotifySound";
            this.checkBoxNotifySound.Size = new System.Drawing.Size(150, 17);
            this.checkBoxNotifySound.TabIndex = 5;
            this.checkBoxNotifySound.Text = "notify me with this sound...";
            this.checkBoxNotifySound.UseVisualStyleBackColor = true;
            this.checkBoxNotifySound.CheckedChanged += new System.EventHandler(this.checkBoxNotifySound_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "When my favor reaches...";
            // 
            // buttonChangeSound
            // 
            this.buttonChangeSound.Location = new System.Drawing.Point(160, 105);
            this.buttonChangeSound.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChangeSound.Name = "buttonChangeSound";
            this.buttonChangeSound.Size = new System.Drawing.Size(74, 21);
            this.buttonChangeSound.TabIndex = 7;
            this.buttonChangeSound.Text = "change";
            this.buttonChangeSound.UseVisualStyleBackColor = true;
            this.buttonChangeSound.Visible = false;
            this.buttonChangeSound.Click += new System.EventHandler(this.buttonChangeSound_Click);
            // 
            // textBoxSoundName
            // 
            this.textBoxSoundName.Location = new System.Drawing.Point(10, 107);
            this.textBoxSoundName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSoundName.Name = "textBoxSoundName";
            this.textBoxSoundName.ReadOnly = true;
            this.textBoxSoundName.Size = new System.Drawing.Size(146, 20);
            this.textBoxSoundName.TabIndex = 8;
            this.textBoxSoundName.Visible = false;
            // 
            // checkBoxNotifyPopup
            // 
            this.checkBoxNotifyPopup.AutoSize = true;
            this.checkBoxNotifyPopup.Location = new System.Drawing.Point(10, 130);
            this.checkBoxNotifyPopup.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxNotifyPopup.Name = "checkBoxNotifyPopup";
            this.checkBoxNotifyPopup.Size = new System.Drawing.Size(141, 17);
            this.checkBoxNotifyPopup.TabIndex = 9;
            this.checkBoxNotifyPopup.Text = "notify me with a popup...";
            this.checkBoxNotifyPopup.UseVisualStyleBackColor = true;
            this.checkBoxNotifyPopup.CheckedChanged += new System.EventHandler(this.checkBoxNotifyPopup_CheckedChanged);
            // 
            // checkBoxPopupPersist
            // 
            this.checkBoxPopupPersist.AutoSize = true;
            this.checkBoxPopupPersist.Location = new System.Drawing.Point(27, 151);
            this.checkBoxPopupPersist.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxPopupPersist.Name = "checkBoxPopupPersist";
            this.checkBoxPopupPersist.Size = new System.Drawing.Size(180, 17);
            this.checkBoxPopupPersist.TabIndex = 10;
            this.checkBoxPopupPersist.Text = "which should persist until clicked";
            this.checkBoxPopupPersist.UseVisualStyleBackColor = true;
            this.checkBoxPopupPersist.Visible = false;
            this.checkBoxPopupPersist.CheckedChanged += new System.EventHandler(this.checkBoxPopupPersist_CheckedChanged);
            // 
            // checkBoxShowFaithSkill
            // 
            this.checkBoxShowFaithSkill.AutoSize = true;
            this.checkBoxShowFaithSkill.Location = new System.Drawing.Point(11, 11);
            this.checkBoxShowFaithSkill.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShowFaithSkill.Name = "checkBoxShowFaithSkill";
            this.checkBoxShowFaithSkill.Size = new System.Drawing.Size(170, 17);
            this.checkBoxShowFaithSkill.TabIndex = 11;
            this.checkBoxShowFaithSkill.Text = "Show me my current faith level";
            this.checkBoxShowFaithSkill.UseVisualStyleBackColor = true;
            this.checkBoxShowFaithSkill.CheckedChanged += new System.EventHandler(this.checkBoxShowFaithSkill_CheckedChanged);
            // 
            // PrayerTimerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 206);
            this.Controls.Add(this.checkBoxShowFaithSkill);
            this.Controls.Add(this.checkBoxPopupPersist);
            this.Controls.Add(this.checkBoxNotifyPopup);
            this.Controls.Add(this.textBoxSoundName);
            this.Controls.Add(this.buttonChangeSound);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxNotifySound);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxFavorWhenMAX);
            this.Controls.Add(this.numericUpDownFavorWhenThis);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrayerTimerOptionsForm";
            this.ShowIcon = false;
            this.Text = "Prayer Timer Options";
            this.Load += new System.EventHandler(this.PrayerTimerOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFavorWhenThis)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownFavorWhenThis;
        private System.Windows.Forms.CheckBox checkBoxFavorWhenMAX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxNotifySound;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonChangeSound;
        private System.Windows.Forms.TextBox textBoxSoundName;
        private System.Windows.Forms.CheckBox checkBoxNotifyPopup;
        private System.Windows.Forms.CheckBox checkBoxPopupPersist;
        private System.Windows.Forms.CheckBox checkBoxShowFaithSkill;
    }
}