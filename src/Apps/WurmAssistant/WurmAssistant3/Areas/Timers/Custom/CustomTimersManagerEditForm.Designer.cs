using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Custom
{
    partial class CustomTimersManagerEditForm
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
            this.checkBoxUptimeReset = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCond = new System.Windows.Forms.TextBox();
            this.checkBoxAsRegex = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxLogType = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnElapsedTime = new System.Windows.Forms.RadioButton();
            this.rbnCooldown = new System.Windows.Forms.RadioButton();
            this.timeInputUControl2 = new AldursLab.WurmAssistant3.Utils.WinForms.TimeSpanInput();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxUptimeReset
            // 
            this.checkBoxUptimeReset.AutoSize = true;
            this.checkBoxUptimeReset.Location = new System.Drawing.Point(12, 236);
            this.checkBoxUptimeReset.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxUptimeReset.Name = "checkBoxUptimeReset";
            this.checkBoxUptimeReset.Size = new System.Drawing.Size(246, 17);
            this.checkBoxUptimeReset.TabIndex = 5;
            this.checkBoxUptimeReset.Text = "and will be reset on each 24hour server uptime";
            this.checkBoxUptimeReset.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 44);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "and it should start if this condition is found...";
            // 
            // textBoxCond
            // 
            this.textBoxCond.Location = new System.Drawing.Point(11, 60);
            this.textBoxCond.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCond.Name = "textBoxCond";
            this.textBoxCond.Size = new System.Drawing.Size(320, 20);
            this.textBoxCond.TabIndex = 1;
            // 
            // checkBoxAsRegex
            // 
            this.checkBoxAsRegex.AutoSize = true;
            this.checkBoxAsRegex.Location = new System.Drawing.Point(334, 61);
            this.checkBoxAsRegex.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAsRegex.Name = "checkBoxAsRegex";
            this.checkBoxAsRegex.Size = new System.Drawing.Size(107, 17);
            this.checkBoxAsRegex.TabIndex = 2;
            this.checkBoxAsRegex.Text = "as Regex pattern";
            this.checkBoxAsRegex.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "in this log...";
            // 
            // comboBoxLogType
            // 
            this.comboBoxLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogType.FormattingEnabled = true;
            this.comboBoxLogType.Location = new System.Drawing.Point(11, 97);
            this.comboBoxLogType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxLogType.Name = "comboBoxLogType";
            this.comboBoxLogType.Size = new System.Drawing.Size(107, 21);
            this.comboBoxLogType.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(387, 222);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 32);
            this.button1.TabIndex = 6;
            this.button1.Text = "Abracadabra!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "My timer shall be called...";
            // 
            // textBoxTimerName
            // 
            this.textBoxTimerName.Location = new System.Drawing.Point(9, 24);
            this.textBoxTimerName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(232, 20);
            this.textBoxTimerName.TabIndex = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 150;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 150;
            this.toolTip1.ReshowDelay = 30;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnElapsedTime);
            this.groupBox1.Controls.Add(this.rbnCooldown);
            this.groupBox1.Controls.Add(this.timeInputUControl2);
            this.groupBox1.Location = new System.Drawing.Point(11, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 108);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "and it looks like...";
            // 
            // rbnElapsedTime
            // 
            this.rbnElapsedTime.AutoSize = true;
            this.rbnElapsedTime.Location = new System.Drawing.Point(22, 19);
            this.rbnElapsedTime.Name = "rbnElapsedTime";
            this.rbnElapsedTime.Size = new System.Drawing.Size(85, 17);
            this.rbnElapsedTime.TabIndex = 18;
            this.rbnElapsedTime.Text = "Elapsed time";
            this.rbnElapsedTime.UseVisualStyleBackColor = true;
            // 
            // rbnCooldown
            // 
            this.rbnCooldown.AutoSize = true;
            this.rbnCooldown.Checked = true;
            this.rbnCooldown.Location = new System.Drawing.Point(22, 42);
            this.rbnCooldown.Name = "rbnCooldown";
            this.rbnCooldown.Size = new System.Drawing.Size(138, 17);
            this.rbnCooldown.TabIndex = 17;
            this.rbnCooldown.TabStop = true;
            this.rbnCooldown.Text = "Cooldown and will last...";
            this.rbnCooldown.UseVisualStyleBackColor = true;
            // 
            // timeInputUControl2
            // 
            this.timeInputUControl2.Location = new System.Drawing.Point(69, 53);
            this.timeInputUControl2.Margin = new System.Windows.Forms.Padding(2);
            this.timeInputUControl2.Name = "timeInputUControl2";
            this.timeInputUControl2.Size = new System.Drawing.Size(231, 45);
            this.timeInputUControl2.TabIndex = 15;
            this.timeInputUControl2.Value = System.TimeSpan.Parse("00:00:00");
            // 
            // CustomTimersManagerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 264);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxTimerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxUptimeReset);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxLogType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxAsRegex);
            this.Controls.Add(this.textBoxCond);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomTimersManagerEditForm";
            this.ShowIcon = false;
            this.Text = "Create custom timer...";
            this.Load += new System.EventHandler(this.CustomTimersManagerEditWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUptimeReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCond;
        private System.Windows.Forms.CheckBox checkBoxAsRegex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxLogType;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnElapsedTime;
        private System.Windows.Forms.RadioButton rbnCooldown;
        private TimeSpanInput timeInputUControl2;
    }
}