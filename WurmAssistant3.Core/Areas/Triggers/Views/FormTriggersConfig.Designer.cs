namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views
{
    partial class FormTriggersConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTriggersConfig));
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TriggersListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnTriggerName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTriggerType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTriggerCondition = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTriggerLogs = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTriggerCdRem = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTriggerActive = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnSound = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnPopup = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label5 = new System.Windows.Forms.Label();
            this.buttonManageSNDBank = new System.Windows.Forms.Button();
            this.buttonMute = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TriggersListView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdd.Location = new System.Drawing.Point(69, 351);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(68, 38);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemove.Location = new System.Drawing.Point(214, 351);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(68, 38);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TriggersListView);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.buttonManageSNDBank);
            this.groupBox1.Controls.Add(this.buttonMute);
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(826, 394);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "All your triggers";
            // 
            // TriggersListView
            // 
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerName);
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerType);
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerCondition);
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerLogs);
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerCdRem);
            this.TriggersListView.AllColumns.Add(this.olvColumnTriggerActive);
            this.TriggersListView.AllColumns.Add(this.olvColumnSound);
            this.TriggersListView.AllColumns.Add(this.olvColumnPopup);
            this.TriggersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TriggersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnTriggerName,
            this.olvColumnTriggerType,
            this.olvColumnTriggerCondition,
            this.olvColumnTriggerLogs,
            this.olvColumnTriggerCdRem,
            this.olvColumnTriggerActive,
            this.olvColumnSound,
            this.olvColumnPopup});
            this.TriggersListView.FullRowSelect = true;
            this.TriggersListView.GridLines = true;
            this.TriggersListView.Location = new System.Drawing.Point(5, 18);
            this.TriggersListView.Name = "TriggersListView";
            this.TriggersListView.ShowGroups = false;
            this.TriggersListView.Size = new System.Drawing.Size(816, 328);
            this.TriggersListView.TabIndex = 13;
            this.TriggersListView.UseCellFormatEvents = true;
            this.TriggersListView.UseCompatibleStateImageBehavior = false;
            this.TriggersListView.View = System.Windows.Forms.View.Details;
            this.TriggersListView.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.TriggersListView_CellClick);
            this.TriggersListView.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.TriggersListView_FormatCell);
            this.TriggersListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.TriggersListView_FormatRow);
            this.TriggersListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TriggersListView_KeyUp);
            this.TriggersListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TriggersListView_MouseClick);
            this.TriggersListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TriggersListView_MouseDoubleClick);
            // 
            // olvColumnTriggerName
            // 
            this.olvColumnTriggerName.AspectName = "Name";
            this.olvColumnTriggerName.Text = "Name";
            this.olvColumnTriggerName.Width = 120;
            // 
            // olvColumnTriggerType
            // 
            this.olvColumnTriggerType.AspectName = "TypeAspect";
            this.olvColumnTriggerType.Text = "Type";
            this.olvColumnTriggerType.Width = 90;
            // 
            // olvColumnTriggerCondition
            // 
            this.olvColumnTriggerCondition.AspectName = "ConditionAspect";
            this.olvColumnTriggerCondition.Text = "Condition";
            this.olvColumnTriggerCondition.Width = 300;
            // 
            // olvColumnTriggerLogs
            // 
            this.olvColumnTriggerLogs.AspectName = "LogTypesAspect";
            this.olvColumnTriggerLogs.Text = "Logs";
            this.olvColumnTriggerLogs.Width = 120;
            // 
            // olvColumnTriggerCdRem
            // 
            this.olvColumnTriggerCdRem.AspectName = "CooldownRemainingAspect";
            this.olvColumnTriggerCdRem.Text = "Cooldown";
            // 
            // olvColumnTriggerActive
            // 
            this.olvColumnTriggerActive.AspectName = "Active";
            this.olvColumnTriggerActive.Text = "Active";
            this.olvColumnTriggerActive.Width = 45;
            // 
            // olvColumnSound
            // 
            this.olvColumnSound.AspectName = "";
            this.olvColumnSound.Text = "S";
            this.olvColumnSound.Width = 20;
            // 
            // olvColumnPopup
            // 
            this.olvColumnPopup.AspectName = "";
            this.olvColumnPopup.Text = "P";
            this.olvColumnPopup.Width = 20;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(615, 356);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 26);
            this.label5.TabIndex = 12;
            this.label5.Text = "Right-click a trigger to\r\nenable/disable it";
            // 
            // buttonManageSNDBank
            // 
            this.buttonManageSNDBank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonManageSNDBank.Location = new System.Drawing.Point(743, 351);
            this.buttonManageSNDBank.Margin = new System.Windows.Forms.Padding(2);
            this.buttonManageSNDBank.Name = "buttonManageSNDBank";
            this.buttonManageSNDBank.Size = new System.Drawing.Size(78, 38);
            this.buttonManageSNDBank.TabIndex = 11;
            this.buttonManageSNDBank.Text = "My sounds";
            this.buttonManageSNDBank.UseVisualStyleBackColor = true;
            this.buttonManageSNDBank.Click += new System.EventHandler(this.buttonManageSNDBank_Click);
            // 
            // buttonMute
            // 
            this.buttonMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMute.Image = global::AldursLab.WurmAssistant3.Core.Properties.Resources.SoundEnabledSmall;
            this.buttonMute.Location = new System.Drawing.Point(4, 351);
            this.buttonMute.Margin = new System.Windows.Forms.Padding(2);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(39, 38);
            this.buttonMute.TabIndex = 8;
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(142, 351);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(68, 38);
            this.buttonEdit.TabIndex = 7;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 10;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(7);
            this.panel1.Size = new System.Drawing.Size(840, 408);
            this.panel1.TabIndex = 6;
            // 
            // FormTriggersConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 408);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(604, 446);
            this.Name = "FormTriggersConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Triggers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSoundNotifyConfig_FormClosing);
            this.Load += new System.EventHandler(this.FormSoundNotifyConfig_Load);
            this.VisibleChanged += new System.EventHandler(this.FormTriggersConfig_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TriggersListView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonEdit;
        public System.Windows.Forms.Button buttonMute;
        private System.Windows.Forms.Button buttonManageSNDBank;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private BrightIdeasSoftware.ObjectListView TriggersListView;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerName;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerType;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerCondition;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerLogs;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerCdRem;
        private BrightIdeasSoftware.OLVColumn olvColumnTriggerActive;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private BrightIdeasSoftware.OLVColumn olvColumnSound;
        private BrightIdeasSoftware.OLVColumn olvColumnPopup;

    }
}