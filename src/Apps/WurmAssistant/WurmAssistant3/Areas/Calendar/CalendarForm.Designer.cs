using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    partial class CalendarForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonRealTime = new System.Windows.Forms.RadioButton();
            this.radioButtonWurmTime = new System.Windows.Forms.RadioButton();
            this.checkBoxSoundWarning = new System.Windows.Forms.CheckBox();
            this.checkBoxPopupWarning = new System.Windows.Forms.CheckBox();
            this.buttonChooseSeasons = new System.Windows.Forms.Button();
            this.buttonClearSound = new System.Windows.Forms.Button();
            this.buttonChooseSound = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxChosenSeasons = new System.Windows.Forms.TextBox();
            this.textBoxChosenSound = new System.Windows.Forms.TextBox();
            this.listViewNFSeasons = new ListViewNf();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxChooseServer = new System.Windows.Forms.ComboBox();
            this.textBoxWurmDate = new System.Windows.Forms.TextBox();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.buttonModSeasonList = new System.Windows.Forms.Button();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.labelDisplayTimeMode = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.radioButtonRealTime);
            this.groupBox1.Controls.Add(this.radioButtonWurmTime);
            this.groupBox1.Location = new System.Drawing.Point(299, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(128, 72);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display time as...";
            // 
            // radioButtonRealTime
            // 
            this.radioButtonRealTime.AutoSize = true;
            this.radioButtonRealTime.Location = new System.Drawing.Point(4, 39);
            this.radioButtonRealTime.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonRealTime.Name = "radioButtonRealTime";
            this.radioButtonRealTime.Size = new System.Drawing.Size(73, 17);
            this.radioButtonRealTime.TabIndex = 1;
            this.radioButtonRealTime.TabStop = true;
            this.radioButtonRealTime.Text = "Real Time";
            this.radioButtonRealTime.UseVisualStyleBackColor = true;
            this.radioButtonRealTime.CheckedChanged += new System.EventHandler(this.radioButtonRealTime_CheckedChanged);
            // 
            // radioButtonWurmTime
            // 
            this.radioButtonWurmTime.AutoSize = true;
            this.radioButtonWurmTime.Location = new System.Drawing.Point(4, 17);
            this.radioButtonWurmTime.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonWurmTime.Name = "radioButtonWurmTime";
            this.radioButtonWurmTime.Size = new System.Drawing.Size(79, 17);
            this.radioButtonWurmTime.TabIndex = 0;
            this.radioButtonWurmTime.TabStop = true;
            this.radioButtonWurmTime.Text = "Wurm Time";
            this.radioButtonWurmTime.UseVisualStyleBackColor = true;
            this.radioButtonWurmTime.CheckedChanged += new System.EventHandler(this.radioButtonWurmTime_CheckedChanged);
            // 
            // checkBoxSoundWarning
            // 
            this.checkBoxSoundWarning.AutoSize = true;
            this.checkBoxSoundWarning.Location = new System.Drawing.Point(4, 46);
            this.checkBoxSoundWarning.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSoundWarning.Name = "checkBoxSoundWarning";
            this.checkBoxSoundWarning.Size = new System.Drawing.Size(57, 17);
            this.checkBoxSoundWarning.TabIndex = 2;
            this.checkBoxSoundWarning.Text = "Sound";
            this.checkBoxSoundWarning.UseVisualStyleBackColor = true;
            this.checkBoxSoundWarning.CheckedChanged += new System.EventHandler(this.checkBoxSoundWarning_CheckedChanged);
            // 
            // checkBoxPopupWarning
            // 
            this.checkBoxPopupWarning.AutoSize = true;
            this.checkBoxPopupWarning.Location = new System.Drawing.Point(4, 24);
            this.checkBoxPopupWarning.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxPopupWarning.Name = "checkBoxPopupWarning";
            this.checkBoxPopupWarning.Size = new System.Drawing.Size(186, 17);
            this.checkBoxPopupWarning.TabIndex = 3;
            this.checkBoxPopupWarning.Text = "Popup (bottom right of the screen)";
            this.checkBoxPopupWarning.UseVisualStyleBackColor = true;
            this.checkBoxPopupWarning.CheckedChanged += new System.EventHandler(this.checkBoxPopupWarning_CheckedChanged);
            // 
            // buttonChooseSeasons
            // 
            this.buttonChooseSeasons.Location = new System.Drawing.Point(219, 17);
            this.buttonChooseSeasons.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChooseSeasons.Name = "buttonChooseSeasons";
            this.buttonChooseSeasons.Size = new System.Drawing.Size(188, 22);
            this.buttonChooseSeasons.TabIndex = 4;
            this.buttonChooseSeasons.Text = "Choose seasons of interest...";
            this.buttonChooseSeasons.UseVisualStyleBackColor = true;
            this.buttonChooseSeasons.Click += new System.EventHandler(this.buttonChooseSeasons_Click);
            // 
            // buttonClearSound
            // 
            this.buttonClearSound.Location = new System.Drawing.Point(145, 68);
            this.buttonClearSound.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClearSound.Name = "buttonClearSound";
            this.buttonClearSound.Size = new System.Drawing.Size(48, 23);
            this.buttonClearSound.TabIndex = 15;
            this.buttonClearSound.Text = "clear";
            this.buttonClearSound.UseVisualStyleBackColor = true;
            this.buttonClearSound.Click += new System.EventHandler(this.buttonClearSound_Click);
            // 
            // buttonChooseSound
            // 
            this.buttonChooseSound.Location = new System.Drawing.Point(4, 68);
            this.buttonChooseSound.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChooseSound.Name = "buttonChooseSound";
            this.buttonChooseSound.Size = new System.Drawing.Size(138, 23);
            this.buttonChooseSound.TabIndex = 14;
            this.buttonChooseSound.Text = "Choose sound";
            this.buttonChooseSound.UseVisualStyleBackColor = true;
            this.buttonChooseSound.Click += new System.EventHandler(this.buttonChooseSound_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.textBoxChosenSeasons);
            this.groupBox2.Controls.Add(this.checkBoxPopupWarning);
            this.groupBox2.Controls.Add(this.textBoxChosenSound);
            this.groupBox2.Controls.Add(this.buttonChooseSeasons);
            this.groupBox2.Controls.Add(this.buttonChooseSound);
            this.groupBox2.Controls.Add(this.checkBoxSoundWarning);
            this.groupBox2.Controls.Add(this.buttonClearSound);
            this.groupBox2.Location = new System.Drawing.Point(2, 105);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(424, 122);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Season reminders";
            // 
            // textBoxChosenSeasons
            // 
            this.textBoxChosenSeasons.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxChosenSeasons.Location = new System.Drawing.Point(219, 43);
            this.textBoxChosenSeasons.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxChosenSeasons.Multiline = true;
            this.textBoxChosenSeasons.Name = "textBoxChosenSeasons";
            this.textBoxChosenSeasons.ReadOnly = true;
            this.textBoxChosenSeasons.Size = new System.Drawing.Size(189, 78);
            this.textBoxChosenSeasons.TabIndex = 17;
            // 
            // textBoxChosenSound
            // 
            this.textBoxChosenSound.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxChosenSound.Location = new System.Drawing.Point(4, 95);
            this.textBoxChosenSound.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxChosenSound.Name = "textBoxChosenSound";
            this.textBoxChosenSound.ReadOnly = true;
            this.textBoxChosenSound.Size = new System.Drawing.Size(189, 23);
            this.textBoxChosenSound.TabIndex = 16;
            // 
            // listViewNFSeasons
            // 
            this.listViewNFSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewNFSeasons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewNFSeasons.GridLines = true;
            this.listViewNFSeasons.Location = new System.Drawing.Point(9, 28);
            this.listViewNFSeasons.Margin = new System.Windows.Forms.Padding(2);
            this.listViewNFSeasons.Name = "listViewNFSeasons";
            this.listViewNFSeasons.Size = new System.Drawing.Size(454, 305);
            this.listViewNFSeasons.TabIndex = 17;
            this.listViewNFSeasons.UseCompatibleStateImageBehavior = false;
            this.listViewNFSeasons.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Plant";
            this.columnHeader4.Width = 110;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Starts in...";
            this.columnHeader5.Width = 240;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Lasts for...";
            this.columnHeader6.Width = 200;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.comboBoxChooseServer);
            this.groupBox3.Controls.Add(this.textBoxWurmDate);
            this.groupBox3.Location = new System.Drawing.Point(1, 1);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(288, 99);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current Wurm Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Track seasons for this server:";
            // 
            // comboBoxChooseServer
            // 
            this.comboBoxChooseServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChooseServer.Enabled = false;
            this.comboBoxChooseServer.FormattingEnabled = true;
            this.comboBoxChooseServer.Location = new System.Drawing.Point(156, 24);
            this.comboBoxChooseServer.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxChooseServer.MaxDropDownItems = 12;
            this.comboBoxChooseServer.Name = "comboBoxChooseServer";
            this.comboBoxChooseServer.Size = new System.Drawing.Size(128, 21);
            this.comboBoxChooseServer.TabIndex = 19;
            this.comboBoxChooseServer.SelectedIndexChanged += new System.EventHandler(this.comboBoxChooseServer_SelectedIndexChanged);
            // 
            // textBoxWurmDate
            // 
            this.textBoxWurmDate.Location = new System.Drawing.Point(5, 49);
            this.textBoxWurmDate.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxWurmDate.Multiline = true;
            this.textBoxWurmDate.Name = "textBoxWurmDate";
            this.textBoxWurmDate.ReadOnly = true;
            this.textBoxWurmDate.Size = new System.Drawing.Size(279, 46);
            this.textBoxWurmDate.TabIndex = 0;
            // 
            // panelOptions
            // 
            this.panelOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelOptions.Controls.Add(this.buttonModSeasonList);
            this.panelOptions.Controls.Add(this.groupBox2);
            this.panelOptions.Controls.Add(this.groupBox3);
            this.panelOptions.Controls.Add(this.groupBox1);
            this.panelOptions.Location = new System.Drawing.Point(9, 102);
            this.panelOptions.Margin = new System.Windows.Forms.Padding(2);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(431, 231);
            this.panelOptions.TabIndex = 19;
            this.panelOptions.Visible = false;
            // 
            // buttonModSeasonList
            // 
            this.buttonModSeasonList.Location = new System.Drawing.Point(299, 78);
            this.buttonModSeasonList.Margin = new System.Windows.Forms.Padding(2);
            this.buttonModSeasonList.Name = "buttonModSeasonList";
            this.buttonModSeasonList.Size = new System.Drawing.Size(110, 22);
            this.buttonModSeasonList.TabIndex = 18;
            this.buttonModSeasonList.Text = "Mod the season list";
            this.buttonModSeasonList.UseVisualStyleBackColor = true;
            this.buttonModSeasonList.Click += new System.EventHandler(this.buttonModSeasonList_Click);
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonConfigure.Location = new System.Drawing.Point(9, 337);
            this.buttonConfigure.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(105, 28);
            this.buttonConfigure.TabIndex = 20;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // labelDisplayTimeMode
            // 
            this.labelDisplayTimeMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDisplayTimeMode.AutoSize = true;
            this.labelDisplayTimeMode.Location = new System.Drawing.Point(128, 344);
            this.labelDisplayTimeMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDisplayTimeMode.Name = "labelDisplayTimeMode";
            this.labelDisplayTimeMode.Size = new System.Drawing.Size(98, 13);
            this.labelDisplayTimeMode.TabIndex = 21;
            this.labelDisplayTimeMode.Text = "Showing times as...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Note: Times are a best guess, actual seasons are a bit random.";
            // 
            // FormCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 376);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelDisplayTimeMode);
            this.Controls.Add(this.buttonConfigure);
            this.Controls.Add(this.panelOptions);
            this.Controls.Add(this.listViewNFSeasons);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CalendarForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Season Calendar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCalendar_FormClosing);
            this.Load += new System.EventHandler(this.FormCalendar_Load);
            this.Resize += new System.EventHandler(this.FormCalendar_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panelOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonRealTime;
        private System.Windows.Forms.RadioButton radioButtonWurmTime;
        private System.Windows.Forms.CheckBox checkBoxSoundWarning;
        private System.Windows.Forms.CheckBox checkBoxPopupWarning;
        private System.Windows.Forms.Button buttonChooseSeasons;
        private System.Windows.Forms.Button buttonClearSound;
        private System.Windows.Forms.Button buttonChooseSound;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxChosenSeasons;
        private System.Windows.Forms.TextBox textBoxChosenSound;
        private ListViewNf listViewNFSeasons;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxWurmDate;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.Button buttonConfigure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxChooseServer;
        private System.Windows.Forms.Label labelDisplayTimeMode;
        private System.Windows.Forms.Button buttonModSeasonList;
        private System.Windows.Forms.Label label2;
    }
}