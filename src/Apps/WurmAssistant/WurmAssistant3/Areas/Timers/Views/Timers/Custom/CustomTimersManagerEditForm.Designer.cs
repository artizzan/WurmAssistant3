using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Views.Timers.Custom
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
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timeInputUControl2 = new TimeSpanInput();
            this.SuspendLayout();
            // 
            // checkBoxUptimeReset
            // 
            this.checkBoxUptimeReset.AutoSize = true;
            this.checkBoxUptimeReset.Location = new System.Drawing.Point(15, 227);
            this.checkBoxUptimeReset.Name = "checkBoxUptimeReset";
            this.checkBoxUptimeReset.Size = new System.Drawing.Size(326, 21);
            this.checkBoxUptimeReset.TabIndex = 5;
            this.checkBoxUptimeReset.Text = "and will be reset on each 24hour server uptime";
            this.checkBoxUptimeReset.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "and it should start if this condition is found...";
            // 
            // textBoxCond
            // 
            this.textBoxCond.Location = new System.Drawing.Point(15, 74);
            this.textBoxCond.Name = "textBoxCond";
            this.textBoxCond.Size = new System.Drawing.Size(425, 22);
            this.textBoxCond.TabIndex = 1;
            // 
            // checkBoxAsRegex
            // 
            this.checkBoxAsRegex.AutoSize = true;
            this.checkBoxAsRegex.Location = new System.Drawing.Point(446, 75);
            this.checkBoxAsRegex.Name = "checkBoxAsRegex";
            this.checkBoxAsRegex.Size = new System.Drawing.Size(138, 21);
            this.checkBoxAsRegex.TabIndex = 2;
            this.checkBoxAsRegex.Text = "as Regex pattern";
            this.checkBoxAsRegex.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "in this log...";
            // 
            // comboBoxLogType
            // 
            this.comboBoxLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogType.FormattingEnabled = true;
            this.comboBoxLogType.Location = new System.Drawing.Point(15, 119);
            this.comboBoxLogType.Name = "comboBoxLogType";
            this.comboBoxLogType.Size = new System.Drawing.Size(141, 24);
            this.comboBoxLogType.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Cooldown will last...";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(516, 273);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 40);
            this.button1.TabIndex = 6;
            this.button1.Text = "Abracadabra!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "My timer shall be called...";
            // 
            // textBoxNameID
            // 
            this.textBoxTimerName.Location = new System.Drawing.Point(12, 29);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(308, 22);
            this.textBoxTimerName.TabIndex = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 150;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 150;
            this.toolTip1.ReshowDelay = 30;
            // 
            // timeInputUControl2
            // 
            this.timeInputUControl2.Location = new System.Drawing.Point(12, 166);
            this.timeInputUControl2.Name = "timeInputUControl2";
            this.timeInputUControl2.Size = new System.Drawing.Size(308, 55);
            this.timeInputUControl2.TabIndex = 4;
            this.timeInputUControl2.Value = System.TimeSpan.Parse("00:00:00");
            // 
            // CustomTimersManagerEditWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 325);
            this.Controls.Add(this.timeInputUControl2);
            this.Controls.Add(this.textBoxTimerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxLogType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxAsRegex);
            this.Controls.Add(this.textBoxCond);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxUptimeReset);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomTimersManagerEditForm";
            this.ShowIcon = false;
            this.Text = "Create custom timer...";
            this.Load += new System.EventHandler(this.CustomTimersManagerEditWindow_Load);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private TimeSpanInput timeInputUControl2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}