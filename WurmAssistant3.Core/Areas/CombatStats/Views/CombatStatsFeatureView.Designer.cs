namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Views
{
    partial class CombatStatsFeatureView
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
            this.createLiveSessionBtn = new System.Windows.Forms.Button();
            this.wurmCharacterCbox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.historicCharacterCbox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.last30DaysBtn = new System.Windows.Forms.Button();
            this.generateStatsBtn = new System.Windows.Forms.Button();
            this.last7DaysBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.todayBtn = new System.Windows.Forms.Button();
            this.fromDtpick = new System.Windows.Forms.DateTimePicker();
            this.toDtpick = new System.Windows.Forms.DateTimePicker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // createLiveSessionBtn
            // 
            this.createLiveSessionBtn.Location = new System.Drawing.Point(5, 72);
            this.createLiveSessionBtn.Name = "createLiveSessionBtn";
            this.createLiveSessionBtn.Size = new System.Drawing.Size(160, 23);
            this.createLiveSessionBtn.TabIndex = 0;
            this.createLiveSessionBtn.Text = "Create Live Session";
            this.createLiveSessionBtn.UseVisualStyleBackColor = true;
            this.createLiveSessionBtn.Click += new System.EventHandler(this.createLiveSessionBtn_Click);
            // 
            // wurmCharacterCbox
            // 
            this.wurmCharacterCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wurmCharacterCbox.FormattingEnabled = true;
            this.wurmCharacterCbox.Location = new System.Drawing.Point(5, 45);
            this.wurmCharacterCbox.Name = "wurmCharacterCbox";
            this.wurmCharacterCbox.Size = new System.Drawing.Size(160, 21);
            this.wurmCharacterCbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Game character:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.createLiveSessionBtn);
            this.groupBox1.Controls.Add(this.wurmCharacterCbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 111);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Live events monitor";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.last30DaysBtn);
            this.groupBox2.Controls.Add(this.generateStatsBtn);
            this.groupBox2.Controls.Add(this.last7DaysBtn);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.todayBtn);
            this.groupBox2.Controls.Add(this.fromDtpick);
            this.groupBox2.Controls.Add(this.toDtpick);
            this.groupBox2.Controls.Add(this.historicCharacterCbox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(199, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 210);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Combat log parser";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Paste log events for:";
            // 
            // historicCharacterCbox
            // 
            this.historicCharacterCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.historicCharacterCbox.FormattingEnabled = true;
            this.historicCharacterCbox.Location = new System.Drawing.Point(10, 36);
            this.historicCharacterCbox.Name = "historicCharacterCbox";
            this.historicCharacterCbox.Size = new System.Drawing.Size(178, 21);
            this.historicCharacterCbox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "From";
            // 
            // last30DaysBtn
            // 
            this.last30DaysBtn.Location = new System.Drawing.Point(172, 143);
            this.last30DaysBtn.Name = "last30DaysBtn";
            this.last30DaysBtn.Size = new System.Drawing.Size(75, 23);
            this.last30DaysBtn.TabIndex = 19;
            this.last30DaysBtn.Text = "Last 30 days";
            this.last30DaysBtn.UseVisualStyleBackColor = true;
            this.last30DaysBtn.Click += new System.EventHandler(this.last30DaysBtn_Click);
            // 
            // generateStatsBtn
            // 
            this.generateStatsBtn.Location = new System.Drawing.Point(10, 172);
            this.generateStatsBtn.Name = "generateStatsBtn";
            this.generateStatsBtn.Size = new System.Drawing.Size(237, 28);
            this.generateStatsBtn.TabIndex = 12;
            this.generateStatsBtn.Text = "Generate stats";
            this.generateStatsBtn.UseVisualStyleBackColor = true;
            this.generateStatsBtn.Click += new System.EventHandler(this.generateStatsBtn_Click);
            // 
            // last7DaysBtn
            // 
            this.last7DaysBtn.Location = new System.Drawing.Point(91, 143);
            this.last7DaysBtn.Name = "last7DaysBtn";
            this.last7DaysBtn.Size = new System.Drawing.Size(75, 23);
            this.last7DaysBtn.TabIndex = 18;
            this.last7DaysBtn.Text = "Last 7 days";
            this.last7DaysBtn.UseVisualStyleBackColor = true;
            this.last7DaysBtn.Click += new System.EventHandler(this.last7DaysBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "To";
            // 
            // todayBtn
            // 
            this.todayBtn.Location = new System.Drawing.Point(10, 143);
            this.todayBtn.Name = "todayBtn";
            this.todayBtn.Size = new System.Drawing.Size(75, 23);
            this.todayBtn.TabIndex = 17;
            this.todayBtn.Text = "Today";
            this.todayBtn.UseVisualStyleBackColor = true;
            this.todayBtn.Click += new System.EventHandler(this.todayBtn_Click);
            // 
            // fromDtpick
            // 
            this.fromDtpick.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.fromDtpick.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDtpick.Location = new System.Drawing.Point(10, 76);
            this.fromDtpick.Name = "fromDtpick";
            this.fromDtpick.Size = new System.Drawing.Size(237, 20);
            this.fromDtpick.TabIndex = 15;
            // 
            // toDtpick
            // 
            this.toDtpick.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.toDtpick.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDtpick.Location = new System.Drawing.Point(10, 117);
            this.toDtpick.Name = "toDtpick";
            this.toDtpick.Size = new System.Drawing.Size(237, 20);
            this.toDtpick.TabIndex = 16;
            // 
            // CombatAssistFeatureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 229);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CombatStatsFeatureView";
            this.ShowIcon = false;
            this.Text = "Combat Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CombatAssistFeatureView_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button createLiveSessionBtn;
        private System.Windows.Forms.ComboBox wurmCharacterCbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox historicCharacterCbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button last30DaysBtn;
        private System.Windows.Forms.Button generateStatsBtn;
        private System.Windows.Forms.Button last7DaysBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button todayBtn;
        private System.Windows.Forms.DateTimePicker fromDtpick;
        private System.Windows.Forms.DateTimePicker toDtpick;
    }
}