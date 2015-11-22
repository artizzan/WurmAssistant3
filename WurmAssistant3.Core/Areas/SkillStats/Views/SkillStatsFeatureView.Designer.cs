namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Views
{
    partial class SkillStatsFeatureView
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
            this.queryGameCharsCblist = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.startLiveSessionBtn = new System.Windows.Forms.Button();
            this.generateQueryBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.serverGroupCbox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bestSkillsReportBtn = new System.Windows.Forms.Button();
            this.totalSkillReportBtn = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.last30DaysBtn = new System.Windows.Forms.Button();
            this.last7DaysBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.todayBtn = new System.Windows.Forms.Button();
            this.fromDtpick = new System.Windows.Forms.DateTimePicker();
            this.toDtpick = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.liveMonCharacterCbox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // queryGameCharsCblist
            // 
            this.queryGameCharsCblist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.queryGameCharsCblist.CheckOnClick = true;
            this.queryGameCharsCblist.FormattingEnabled = true;
            this.queryGameCharsCblist.Location = new System.Drawing.Point(6, 77);
            this.queryGameCharsCblist.Name = "queryGameCharsCblist";
            this.queryGameCharsCblist.Size = new System.Drawing.Size(364, 259);
            this.queryGameCharsCblist.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Game characters:";
            // 
            // startLiveSessionBtn
            // 
            this.startLiveSessionBtn.Location = new System.Drawing.Point(6, 59);
            this.startLiveSessionBtn.Name = "startLiveSessionBtn";
            this.startLiveSessionBtn.Size = new System.Drawing.Size(178, 23);
            this.startLiveSessionBtn.TabIndex = 3;
            this.startLiveSessionBtn.Text = "Start session";
            this.startLiveSessionBtn.UseVisualStyleBackColor = true;
            this.startLiveSessionBtn.Click += new System.EventHandler(this.startLiveSessionBtn_Click);
            // 
            // generateQueryBtn
            // 
            this.generateQueryBtn.Location = new System.Drawing.Point(9, 131);
            this.generateQueryBtn.Name = "generateQueryBtn";
            this.generateQueryBtn.Size = new System.Drawing.Size(237, 28);
            this.generateQueryBtn.TabIndex = 4;
            this.generateQueryBtn.Text = "Generate query";
            this.generateQueryBtn.UseVisualStyleBackColor = true;
            this.generateQueryBtn.Click += new System.EventHandler(this.generateQueryBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.serverGroupCbox);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.queryGameCharsCblist);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(642, 355);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Skill queries";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Server Group";
            // 
            // serverGroupCbox
            // 
            this.serverGroupCbox.FormattingEnabled = true;
            this.serverGroupCbox.Location = new System.Drawing.Point(6, 32);
            this.serverGroupCbox.Name = "serverGroupCbox";
            this.serverGroupCbox.Size = new System.Drawing.Size(364, 21);
            this.serverGroupCbox.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.bestSkillsReportBtn);
            this.groupBox3.Controls.Add(this.totalSkillReportBtn);
            this.groupBox3.Location = new System.Drawing.Point(376, 192);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 95);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Skill levels query";
            // 
            // bestSkillsReportBtn
            // 
            this.bestSkillsReportBtn.Location = new System.Drawing.Point(6, 53);
            this.bestSkillsReportBtn.Name = "bestSkillsReportBtn";
            this.bestSkillsReportBtn.Size = new System.Drawing.Size(240, 28);
            this.bestSkillsReportBtn.TabIndex = 13;
            this.bestSkillsReportBtn.Text = "Show best skills";
            this.bestSkillsReportBtn.UseVisualStyleBackColor = true;
            this.bestSkillsReportBtn.Click += new System.EventHandler(this.bestSkillsReportBtn_Click);
            // 
            // totalSkillReportBtn
            // 
            this.totalSkillReportBtn.Location = new System.Drawing.Point(6, 19);
            this.totalSkillReportBtn.Name = "totalSkillReportBtn";
            this.totalSkillReportBtn.Size = new System.Drawing.Size(240, 28);
            this.totalSkillReportBtn.TabIndex = 12;
            this.totalSkillReportBtn.Text = "Show total skills";
            this.totalSkillReportBtn.UseVisualStyleBackColor = true;
            this.totalSkillReportBtn.Click += new System.EventHandler(this.totalSkillReportBtn_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.last30DaysBtn);
            this.groupBox4.Controls.Add(this.generateQueryBtn);
            this.groupBox4.Controls.Add(this.last7DaysBtn);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.todayBtn);
            this.groupBox4.Controls.Add(this.fromDtpick);
            this.groupBox4.Controls.Add(this.toDtpick);
            this.groupBox4.Location = new System.Drawing.Point(376, 16);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(260, 170);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Skill gain query";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "From";
            // 
            // last30DaysBtn
            // 
            this.last30DaysBtn.Location = new System.Drawing.Point(171, 102);
            this.last30DaysBtn.Name = "last30DaysBtn";
            this.last30DaysBtn.Size = new System.Drawing.Size(75, 23);
            this.last30DaysBtn.TabIndex = 11;
            this.last30DaysBtn.Text = "Last 30 days";
            this.last30DaysBtn.UseVisualStyleBackColor = true;
            this.last30DaysBtn.Click += new System.EventHandler(this.last30DaysBtn_Click);
            // 
            // last7DaysBtn
            // 
            this.last7DaysBtn.Location = new System.Drawing.Point(90, 102);
            this.last7DaysBtn.Name = "last7DaysBtn";
            this.last7DaysBtn.Size = new System.Drawing.Size(75, 23);
            this.last7DaysBtn.TabIndex = 10;
            this.last7DaysBtn.Text = "Last 7 days";
            this.last7DaysBtn.UseVisualStyleBackColor = true;
            this.last7DaysBtn.Click += new System.EventHandler(this.last7DaysBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "To";
            // 
            // todayBtn
            // 
            this.todayBtn.Location = new System.Drawing.Point(9, 102);
            this.todayBtn.Name = "todayBtn";
            this.todayBtn.Size = new System.Drawing.Size(75, 23);
            this.todayBtn.TabIndex = 9;
            this.todayBtn.Text = "Today";
            this.todayBtn.UseVisualStyleBackColor = true;
            this.todayBtn.Click += new System.EventHandler(this.todayBtn_Click);
            // 
            // fromDtpick
            // 
            this.fromDtpick.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.fromDtpick.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDtpick.Location = new System.Drawing.Point(9, 35);
            this.fromDtpick.Name = "fromDtpick";
            this.fromDtpick.Size = new System.Drawing.Size(237, 20);
            this.fromDtpick.TabIndex = 7;
            // 
            // toDtpick
            // 
            this.toDtpick.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.toDtpick.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDtpick.Location = new System.Drawing.Point(9, 76);
            this.toDtpick.Name = "toDtpick";
            this.toDtpick.Size = new System.Drawing.Size(237, 20);
            this.toDtpick.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.liveMonCharacterCbox);
            this.groupBox2.Controls.Add(this.startLiveSessionBtn);
            this.groupBox2.Location = new System.Drawing.Point(660, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 94);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Live skills monitor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Game character:";
            // 
            // liveMonCharacterCbox
            // 
            this.liveMonCharacterCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.liveMonCharacterCbox.FormattingEnabled = true;
            this.liveMonCharacterCbox.Location = new System.Drawing.Point(6, 32);
            this.liveMonCharacterCbox.Name = "liveMonCharacterCbox";
            this.liveMonCharacterCbox.Size = new System.Drawing.Size(178, 21);
            this.liveMonCharacterCbox.TabIndex = 4;
            // 
            // SkillStatsFeatureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 380);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SkillStatsFeatureView";
            this.ShowIcon = false;
            this.Text = "Skill Stats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkillStatsView_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox queryGameCharsCblist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startLiveSessionBtn;
        private System.Windows.Forms.Button generateQueryBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox liveMonCharacterCbox;
        private System.Windows.Forms.DateTimePicker toDtpick;
        private System.Windows.Forms.DateTimePicker fromDtpick;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button last30DaysBtn;
        private System.Windows.Forms.Button last7DaysBtn;
        private System.Windows.Forms.Button todayBtn;
        private System.Windows.Forms.Button totalSkillReportBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bestSkillsReportBtn;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox serverGroupCbox;
    }
}