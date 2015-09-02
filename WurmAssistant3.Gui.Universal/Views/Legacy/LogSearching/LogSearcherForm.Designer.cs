namespace AldursLab.WurmAssistant3.Gui.Universal.Views.Legacy.LogSearching
{
    partial class LogSearcherForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxSearchKey = new System.Windows.Forms.TextBox();
            this.dateTimePickerTimeFrom = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerTimeTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxSearchType = new System.Windows.Forms.ComboBox();
            this.comboBoxPlayerName = new System.Windows.Forms.ComboBox();
            this.comboBoxLogType = new System.Windows.Forms.ComboBox();
            this.listBoxAllResults = new System.Windows.Forms.ListBox();
            this.labelAllResults = new System.Windows.Forms.Label();
            this.richTextBoxAllLines = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonCommitSearch = new System.Windows.Forms.Button();
            this.buttonCancelSearch = new System.Windows.Forms.Button();
            this.labelPM = new System.Windows.Forms.Label();
            this.textBoxPM = new System.Windows.Forms.TextBox();
            this.labelWorking = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Wurm character:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Log type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Time from:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 51);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Time to:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(332, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Search type:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(332, 51);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Search key to look for:";
            // 
            // textBoxSearchKey
            // 
            this.textBoxSearchKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxSearchKey.Location = new System.Drawing.Point(332, 66);
            this.textBoxSearchKey.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSearchKey.Name = "textBoxSearchKey";
            this.textBoxSearchKey.Size = new System.Drawing.Size(386, 23);
            this.textBoxSearchKey.TabIndex = 6;
            this.textBoxSearchKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchKey_KeyDown);
            this.textBoxSearchKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchKey_KeyUp);
            // 
            // dateTimePickerTimeFrom
            // 
            this.dateTimePickerTimeFrom.CustomFormat = "dd-MM-yyyy hh:mm";
            this.dateTimePickerTimeFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dateTimePickerTimeFrom.Location = new System.Drawing.Point(114, 23);
            this.dateTimePickerTimeFrom.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerTimeFrom.Name = "dateTimePickerTimeFrom";
            this.dateTimePickerTimeFrom.Size = new System.Drawing.Size(199, 23);
            this.dateTimePickerTimeFrom.TabIndex = 2;
            this.dateTimePickerTimeFrom.Value = new System.DateTime(2012, 12, 3, 0, 0, 0, 0);
            this.dateTimePickerTimeFrom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePickerTimeFrom_KeyUp);
            // 
            // dateTimePickerTimeTo
            // 
            this.dateTimePickerTimeTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dateTimePickerTimeTo.Location = new System.Drawing.Point(114, 66);
            this.dateTimePickerTimeTo.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerTimeTo.Name = "dateTimePickerTimeTo";
            this.dateTimePickerTimeTo.Size = new System.Drawing.Size(201, 23);
            this.dateTimePickerTimeTo.TabIndex = 3;
            this.dateTimePickerTimeTo.Value = new System.DateTime(2012, 12, 3, 0, 0, 0, 0);
            this.dateTimePickerTimeTo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePickerTimeTo_KeyUp);
            // 
            // comboBoxSearchType
            // 
            this.comboBoxSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSearchType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxSearchType.FormattingEnabled = true;
            this.comboBoxSearchType.Location = new System.Drawing.Point(332, 21);
            this.comboBoxSearchType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSearchType.Name = "comboBoxSearchType";
            this.comboBoxSearchType.Size = new System.Drawing.Size(231, 25);
            this.comboBoxSearchType.TabIndex = 4;
            this.comboBoxSearchType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSearchType_KeyUp);
            // 
            // comboBoxPlayerName
            // 
            this.comboBoxPlayerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPlayerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxPlayerName.FormattingEnabled = true;
            this.comboBoxPlayerName.Location = new System.Drawing.Point(9, 21);
            this.comboBoxPlayerName.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxPlayerName.MaxDropDownItems = 20;
            this.comboBoxPlayerName.Name = "comboBoxPlayerName";
            this.comboBoxPlayerName.Size = new System.Drawing.Size(92, 25);
            this.comboBoxPlayerName.TabIndex = 0;
            this.comboBoxPlayerName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxPlayerName_KeyUp);
            // 
            // comboBoxLogType
            // 
            this.comboBoxLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxLogType.FormattingEnabled = true;
            this.comboBoxLogType.Location = new System.Drawing.Point(9, 66);
            this.comboBoxLogType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxLogType.MaxDropDownItems = 20;
            this.comboBoxLogType.Name = "comboBoxLogType";
            this.comboBoxLogType.Size = new System.Drawing.Size(92, 25);
            this.comboBoxLogType.TabIndex = 1;
            this.comboBoxLogType.SelectedIndexChanged += new System.EventHandler(this.comboBoxLogType_SelectedIndexChanged);
            this.comboBoxLogType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxLogType_KeyUp);
            // 
            // listBoxAllResults
            // 
            this.listBoxAllResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxAllResults.FormattingEnabled = true;
            this.listBoxAllResults.Location = new System.Drawing.Point(7, 115);
            this.listBoxAllResults.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxAllResults.Name = "listBoxAllResults";
            this.listBoxAllResults.Size = new System.Drawing.Size(147, 368);
            this.listBoxAllResults.TabIndex = 9;
            this.listBoxAllResults.Click += new System.EventHandler(this.listBoxAllResults_Click);
            // 
            // labelAllResults
            // 
            this.labelAllResults.AutoSize = true;
            this.labelAllResults.Location = new System.Drawing.Point(4, 98);
            this.labelAllResults.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAllResults.Name = "labelAllResults";
            this.labelAllResults.Size = new System.Drawing.Size(54, 13);
            this.labelAllResults.TabIndex = 18;
            this.labelAllResults.Text = "All results:";
            // 
            // richTextBoxAllLines
            // 
            this.richTextBoxAllLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxAllLines.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.richTextBoxAllLines.HideSelection = false;
            this.richTextBoxAllLines.Location = new System.Drawing.Point(158, 115);
            this.richTextBoxAllLines.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxAllLines.Name = "richTextBoxAllLines";
            this.richTextBoxAllLines.ReadOnly = true;
            this.richTextBoxAllLines.Size = new System.Drawing.Size(645, 447);
            this.richTextBoxAllLines.TabIndex = 8;
            this.richTextBoxAllLines.Text = "";
            this.richTextBoxAllLines.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxAllLines_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(165, 98);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "All log entries for this time span:";
            // 
            // buttonCommitSearch
            // 
            this.buttonCommitSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCommitSearch.Location = new System.Drawing.Point(7, 514);
            this.buttonCommitSearch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCommitSearch.Name = "buttonCommitSearch";
            this.buttonCommitSearch.Size = new System.Drawing.Size(146, 47);
            this.buttonCommitSearch.TabIndex = 7;
            this.buttonCommitSearch.Text = "Search";
            this.buttonCommitSearch.UseVisualStyleBackColor = true;
            this.buttonCommitSearch.Click += new System.EventHandler(this.buttonCommitSearch_Click);
            // 
            // buttonCancelSearch
            // 
            this.buttonCancelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancelSearch.Location = new System.Drawing.Point(7, 462);
            this.buttonCancelSearch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancelSearch.Name = "buttonCancelSearch";
            this.buttonCancelSearch.Size = new System.Drawing.Size(147, 47);
            this.buttonCancelSearch.TabIndex = 23;
            this.buttonCancelSearch.TabStop = false;
            this.buttonCancelSearch.Text = "Cancel search";
            this.buttonCancelSearch.UseVisualStyleBackColor = true;
            this.buttonCancelSearch.Visible = false;
            this.buttonCancelSearch.Click += new System.EventHandler(this.buttonCancelSearch_Click);
            // 
            // labelPM
            // 
            this.labelPM.AutoSize = true;
            this.labelPM.Location = new System.Drawing.Point(574, 7);
            this.labelPM.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPM.Name = "labelPM";
            this.labelPM.Size = new System.Drawing.Size(69, 13);
            this.labelPM.TabIndex = 25;
            this.labelPM.Text = "PM recipient:";
            this.labelPM.Visible = false;
            // 
            // textBoxPM
            // 
            this.textBoxPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxPM.Location = new System.Drawing.Point(574, 21);
            this.textBoxPM.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPM.Name = "textBoxPM";
            this.textBoxPM.Size = new System.Drawing.Size(144, 23);
            this.textBoxPM.TabIndex = 5;
            this.textBoxPM.TabStop = false;
            this.textBoxPM.Visible = false;
            this.textBoxPM.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxPM_KeyUp);
            // 
            // labelWorking
            // 
            this.labelWorking.AutoSize = true;
            this.labelWorking.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWorking.Location = new System.Drawing.Point(221, 171);
            this.labelWorking.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelWorking.Name = "labelWorking";
            this.labelWorking.Size = new System.Drawing.Size(287, 31);
            this.labelWorking.TabIndex = 26;
            this.labelWorking.Text = "Preparing log entries...";
            this.labelWorking.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCancelSearch);
            this.panel1.Controls.Add(this.labelWorking);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.richTextBoxAllLines);
            this.panel1.Controls.Add(this.labelPM);
            this.panel1.Controls.Add(this.labelAllResults);
            this.panel1.Controls.Add(this.textBoxPM);
            this.panel1.Controls.Add(this.listBoxAllResults);
            this.panel1.Controls.Add(this.comboBoxLogType);
            this.panel1.Controls.Add(this.comboBoxPlayerName);
            this.panel1.Controls.Add(this.buttonCommitSearch);
            this.panel1.Controls.Add(this.comboBoxSearchType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateTimePickerTimeTo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePickerTimeFrom);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.textBoxSearchKey);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.MinimumSize = new System.Drawing.Size(812, 568);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(2);
            this.panel1.Size = new System.Drawing.Size(812, 568);
            this.panel1.TabIndex = 29;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 10;
            // 
            // LogSearcherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(812, 567);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(828, 606);
            this.Name = "LogSearcherForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogSearcherForm_FormClosing);
            this.Load += new System.EventHandler(this.FormLogSearcher_Load);
            this.Resize += new System.EventHandler(this.LogSearcherForm_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxSearchKey;
        private System.Windows.Forms.DateTimePicker dateTimePickerTimeFrom;
        private System.Windows.Forms.DateTimePicker dateTimePickerTimeTo;
        private System.Windows.Forms.ComboBox comboBoxSearchType;
        private System.Windows.Forms.ComboBox comboBoxPlayerName;
        private System.Windows.Forms.ComboBox comboBoxLogType;
        private System.Windows.Forms.ListBox listBoxAllResults;
        private System.Windows.Forms.Label labelAllResults;
        private System.Windows.Forms.RichTextBox richTextBoxAllLines;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonCommitSearch;
        private System.Windows.Forms.Button buttonCancelSearch;
        private System.Windows.Forms.Label labelPM;
        private System.Windows.Forms.TextBox textBoxPM;
        private System.Windows.Forms.Label labelWorking;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
