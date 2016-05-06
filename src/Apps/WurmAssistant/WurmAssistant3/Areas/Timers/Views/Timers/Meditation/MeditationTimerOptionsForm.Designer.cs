namespace AldursLab.WurmAssistant3.Areas.Timers.Views.Timers.Meditation
{
    partial class MeditationTimerOptionsForm
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
            this.components = new System.ComponentModel.Container();
            this.checkBoxRemindSleepBonus = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownPopupDuration = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panelPopupDuration = new System.Windows.Forms.Panel();
            this.checkBoxShowMeditSkill = new System.Windows.Forms.CheckBox();
            this.checkBoxCount = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopupDuration)).BeginInit();
            this.panelPopupDuration.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxRemindSleepBonus
            // 
            this.checkBoxRemindSleepBonus.AutoSize = true;
            this.checkBoxRemindSleepBonus.Location = new System.Drawing.Point(11, 88);
            this.checkBoxRemindSleepBonus.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxRemindSleepBonus.Name = "checkBoxRemindSleepBonus";
            this.checkBoxRemindSleepBonus.Size = new System.Drawing.Size(175, 17);
            this.checkBoxRemindSleepBonus.TabIndex = 0;
            this.checkBoxRemindSleepBonus.Text = "Popup reminder for sleep bonus";
            this.checkBoxRemindSleepBonus.UseVisualStyleBackColor = true;
            this.checkBoxRemindSleepBonus.CheckedChanged += new System.EventHandler(this.checkBoxRemindSleepBonus_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 150;
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 150;
            this.toolTip1.ReshowDelay = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "seconds";
            // 
            // numericUpDownPopupDuration
            // 
            this.numericUpDownPopupDuration.Location = new System.Drawing.Point(92, 6);
            this.numericUpDownPopupDuration.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPopupDuration.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownPopupDuration.Name = "numericUpDownPopupDuration";
            this.numericUpDownPopupDuration.Size = new System.Drawing.Size(38, 20);
            this.numericUpDownPopupDuration.TabIndex = 6;
            this.numericUpDownPopupDuration.ValueChanged += new System.EventHandler(this.numericUpDownPopupDuration_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "which should stay";
            // 
            // panelPopupDuration
            // 
            this.panelPopupDuration.Controls.Add(this.label1);
            this.panelPopupDuration.Controls.Add(this.label2);
            this.panelPopupDuration.Controls.Add(this.numericUpDownPopupDuration);
            this.panelPopupDuration.Location = new System.Drawing.Point(11, 110);
            this.panelPopupDuration.Margin = new System.Windows.Forms.Padding(2);
            this.panelPopupDuration.Name = "panelPopupDuration";
            this.panelPopupDuration.Size = new System.Drawing.Size(194, 30);
            this.panelPopupDuration.TabIndex = 8;
            // 
            // checkBoxShowMeditSkill
            // 
            this.checkBoxShowMeditSkill.AutoSize = true;
            this.checkBoxShowMeditSkill.Location = new System.Drawing.Point(11, 11);
            this.checkBoxShowMeditSkill.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShowMeditSkill.Name = "checkBoxShowMeditSkill";
            this.checkBoxShowMeditSkill.Size = new System.Drawing.Size(198, 17);
            this.checkBoxShowMeditSkill.TabIndex = 9;
            this.checkBoxShowMeditSkill.Text = "Show me my current meditation level";
            this.checkBoxShowMeditSkill.UseVisualStyleBackColor = true;
            this.checkBoxShowMeditSkill.CheckedChanged += new System.EventHandler(this.checkBoxShowMeditSkill_CheckedChanged);
            // 
            // checkBoxCount
            // 
            this.checkBoxCount.AutoSize = true;
            this.checkBoxCount.Location = new System.Drawing.Point(11, 32);
            this.checkBoxCount.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCount.Name = "checkBoxCount";
            this.checkBoxCount.Size = new System.Drawing.Size(176, 30);
            this.checkBoxCount.TabIndex = 10;
            this.checkBoxCount.Text = "Show me number of meditations\r\ndone in this uptime window";
            this.checkBoxCount.UseVisualStyleBackColor = true;
            this.checkBoxCount.CheckedChanged += new System.EventHandler(this.checkBoxCount_CheckedChanged);
            // 
            // MeditationTimerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 169);
            this.Controls.Add(this.checkBoxCount);
            this.Controls.Add(this.checkBoxShowMeditSkill);
            this.Controls.Add(this.panelPopupDuration);
            this.Controls.Add(this.checkBoxRemindSleepBonus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeditationTimerOptionsForm";
            this.ShowIcon = false;
            this.Text = "More meditation timer options";
            this.Load += new System.EventHandler(this.MeditationTimerOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopupDuration)).EndInit();
            this.panelPopupDuration.ResumeLayout(false);
            this.panelPopupDuration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxRemindSleepBonus;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownPopupDuration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelPopupDuration;
        private System.Windows.Forms.CheckBox checkBoxShowMeditSkill;
        private System.Windows.Forms.CheckBox checkBoxCount;
    }
}