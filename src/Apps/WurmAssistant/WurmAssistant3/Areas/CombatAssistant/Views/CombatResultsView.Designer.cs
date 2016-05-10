namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Views
{
    partial class CombatResultsView
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
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn0 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn12 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn19 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn11 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn13 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn14 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn16 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn17 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn18 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn20 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.nameFilterTbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rowHeightNup = new System.Windows.Forms.NumericUpDown();
            this.legendBtn = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toCsvBtn = new System.Windows.Forms.Button();
            this.olvColumn21 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowHeightNup)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn0);
            this.objectListView1.AllColumns.Add(this.olvColumn1);
            this.objectListView1.AllColumns.Add(this.olvColumn2);
            this.objectListView1.AllColumns.Add(this.olvColumn12);
            this.objectListView1.AllColumns.Add(this.olvColumn5);
            this.objectListView1.AllColumns.Add(this.olvColumn3);
            this.objectListView1.AllColumns.Add(this.olvColumn10);
            this.objectListView1.AllColumns.Add(this.olvColumn7);
            this.objectListView1.AllColumns.Add(this.olvColumn8);
            this.objectListView1.AllColumns.Add(this.olvColumn4);
            this.objectListView1.AllColumns.Add(this.olvColumn6);
            this.objectListView1.AllColumns.Add(this.olvColumn9);
            this.objectListView1.AllColumns.Add(this.olvColumn19);
            this.objectListView1.AllColumns.Add(this.olvColumn11);
            this.objectListView1.AllColumns.Add(this.olvColumn13);
            this.objectListView1.AllColumns.Add(this.olvColumn14);
            this.objectListView1.AllColumns.Add(this.olvColumn15);
            this.objectListView1.AllColumns.Add(this.olvColumn16);
            this.objectListView1.AllColumns.Add(this.olvColumn17);
            this.objectListView1.AllColumns.Add(this.olvColumn18);
            this.objectListView1.AllColumns.Add(this.olvColumn20);
            this.objectListView1.AllColumns.Add(this.olvColumn21);
            this.objectListView1.AllowColumnReorder = true;
            this.objectListView1.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn0,
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn12,
            this.olvColumn5,
            this.olvColumn3,
            this.olvColumn10,
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn4,
            this.olvColumn6,
            this.olvColumn9,
            this.olvColumn19,
            this.olvColumn11,
            this.olvColumn13,
            this.olvColumn14,
            this.olvColumn15,
            this.olvColumn16,
            this.olvColumn17,
            this.olvColumn18,
            this.olvColumn20,
            this.olvColumn21});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HeaderWordWrap = true;
            this.objectListView1.SelectedBackColor = System.Drawing.Color.Empty;
            this.objectListView1.SelectedForeColor = System.Drawing.Color.Empty;
            this.objectListView1.Location = new System.Drawing.Point(12, 36);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.RowHeight = 70;
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.Size = new System.Drawing.Size(1259, 411);
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseAlternatingBackColors = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFilterIndicator = true;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn0
            // 
            this.olvColumn0.AspectName = "CombatPair";
            this.olvColumn0.Text = "Pair";
            this.olvColumn0.Width = 160;
            this.olvColumn0.WordWrap = true;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "AttackerName";
            this.olvColumn1.Searchable = false;
            this.olvColumn1.Text = "Attacker";
            this.olvColumn1.Width = 92;
            this.olvColumn1.WordWrap = true;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "DefenderName";
            this.olvColumn2.Searchable = false;
            this.olvColumn2.Text = "Defender";
            this.olvColumn2.Width = 109;
            this.olvColumn2.WordWrap = true;
            // 
            // olvColumn12
            // 
            this.olvColumn12.AspectName = "SlainCount";
            this.olvColumn12.Searchable = false;
            this.olvColumn12.Text = "Slain count";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "TargetPrefs";
            this.olvColumn5.Searchable = false;
            this.olvColumn5.Text = "Target choices";
            this.olvColumn5.Width = 121;
            this.olvColumn5.WordWrap = true;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "DamageCaused";
            this.olvColumn3.Searchable = false;
            this.olvColumn3.Text = "Damage caused / Attack Strengths";
            this.olvColumn3.Width = 200;
            this.olvColumn3.WordWrap = true;
            // 
            // olvColumn10
            // 
            this.olvColumn10.AspectName = "WeaponSpellAttacks";
            this.olvColumn10.Searchable = false;
            this.olvColumn10.Text = "Spell triggers";
            this.olvColumn10.Width = 116;
            this.olvColumn10.WordWrap = true;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "Misses";
            this.olvColumn7.Searchable = false;
            this.olvColumn7.Text = "Misses";
            this.olvColumn7.Width = 55;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "GlancingBlows";
            this.olvColumn8.Searchable = false;
            this.olvColumn8.Text = "Glancing blows";
            this.olvColumn8.Width = 91;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Parries";
            this.olvColumn4.Searchable = false;
            this.olvColumn4.Text = "Parried";
            this.olvColumn4.Width = 106;
            this.olvColumn4.WordWrap = true;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Evasions";
            this.olvColumn6.Searchable = false;
            this.olvColumn6.Text = "Evaded";
            this.olvColumn6.Width = 88;
            this.olvColumn6.WordWrap = true;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "ShieldBlocks";
            this.olvColumn9.Searchable = false;
            this.olvColumn9.Text = "Shield blocked";
            this.olvColumn9.Width = 78;
            // 
            // olvColumn19
            // 
            this.olvColumn19.AspectName = "TotalHits";
            this.olvColumn19.Searchable = false;
            this.olvColumn19.Text = "Total hits";
            // 
            // olvColumn11
            // 
            this.olvColumn11.AspectName = "TotalMainActorAttacks";
            this.olvColumn11.Searchable = false;
            this.olvColumn11.Text = "Total attacks";
            // 
            // olvColumn13
            // 
            this.olvColumn13.AspectName = "HitRatio";
            this.olvColumn13.Searchable = false;
            this.olvColumn13.Text = "Hit ratio";
            // 
            // olvColumn14
            // 
            this.olvColumn14.AspectName = "MissRatio";
            this.olvColumn14.Searchable = false;
            this.olvColumn14.Text = "Miss ratio";
            // 
            // olvColumn15
            // 
            this.olvColumn15.AspectName = "GlanceRatio";
            this.olvColumn15.Searchable = false;
            this.olvColumn15.Text = "Glance ratio";
            // 
            // olvColumn16
            // 
            this.olvColumn16.AspectName = "BlockRatio";
            this.olvColumn16.Searchable = false;
            this.olvColumn16.Text = "Blocked ratio";
            // 
            // olvColumn17
            // 
            this.olvColumn17.AspectName = "ParryRatio";
            this.olvColumn17.Searchable = false;
            this.olvColumn17.Text = "Parried ratio";
            // 
            // olvColumn18
            // 
            this.olvColumn18.AspectName = "EvadeRatio";
            this.olvColumn18.Searchable = false;
            this.olvColumn18.Text = "Evaded ratio";
            // 
            // olvColumn20
            // 
            this.olvColumn20.AspectName = "FightingSkillGained";
            this.olvColumn20.Text = "Fighting skill gained";
            // 
            // nameFilterTbox
            // 
            this.nameFilterTbox.Location = new System.Drawing.Point(93, 10);
            this.nameFilterTbox.Name = "nameFilterTbox";
            this.nameFilterTbox.Size = new System.Drawing.Size(161, 20);
            this.nameFilterTbox.TabIndex = 1;
            this.nameFilterTbox.TextChanged += new System.EventHandler(this.nameFilterTbox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter by name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Row height:";
            // 
            // rowHeightNup
            // 
            this.rowHeightNup.Location = new System.Drawing.Point(347, 10);
            this.rowHeightNup.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.rowHeightNup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rowHeightNup.Name = "rowHeightNup";
            this.rowHeightNup.Size = new System.Drawing.Size(59, 20);
            this.rowHeightNup.TabIndex = 4;
            this.rowHeightNup.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rowHeightNup.ValueChanged += new System.EventHandler(this.rowHeightNup_ValueChanged);
            // 
            // legendBtn
            // 
            this.legendBtn.Location = new System.Drawing.Point(436, 8);
            this.legendBtn.Name = "legendBtn";
            this.legendBtn.Size = new System.Drawing.Size(75, 23);
            this.legendBtn.TabIndex = 5;
            this.legendBtn.Text = "Legend";
            this.legendBtn.UseVisualStyleBackColor = true;
            this.legendBtn.Click += new System.EventHandler(this.legendBtn_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.FileName = "combat.csv";
            this.saveFileDialog.Filter = "CSV Files|*.csv";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // toCsvBtn
            // 
            this.toCsvBtn.Location = new System.Drawing.Point(517, 8);
            this.toCsvBtn.Name = "toCsvBtn";
            this.toCsvBtn.Size = new System.Drawing.Size(75, 23);
            this.toCsvBtn.TabIndex = 6;
            this.toCsvBtn.Text = "To CSV";
            this.toCsvBtn.UseVisualStyleBackColor = true;
            this.toCsvBtn.Click += new System.EventHandler(this.toCsvBtn_Click);
            // 
            // olvColumn21
            // 
            this.olvColumn21.AspectName = "FightingSkillGainedPerKill";
            this.olvColumn21.Text = "Fighting skill per kill";
            // 
            // CombatResultsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 459);
            this.Controls.Add(this.toCsvBtn);
            this.Controls.Add(this.legendBtn);
            this.Controls.Add(this.rowHeightNup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameFilterTbox);
            this.Controls.Add(this.objectListView1);
            this.Name = "CombatResultsView";
            this.Text = "CombatResultsView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CombatResultsView_FormClosing);
            this.Load += new System.EventHandler(this.CombatResultsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowHeightNup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private System.Windows.Forms.Timer refreshTimer;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn10;
        private BrightIdeasSoftware.OLVColumn olvColumn0;
        private BrightIdeasSoftware.OLVColumn olvColumn11;
        private BrightIdeasSoftware.OLVColumn olvColumn13;
        private BrightIdeasSoftware.OLVColumn olvColumn14;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private BrightIdeasSoftware.OLVColumn olvColumn16;
        private BrightIdeasSoftware.OLVColumn olvColumn17;
        private BrightIdeasSoftware.OLVColumn olvColumn18;
        private BrightIdeasSoftware.OLVColumn olvColumn19;
        private System.Windows.Forms.TextBox nameFilterTbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown rowHeightNup;
        private System.Windows.Forms.Button legendBtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button toCsvBtn;
        private BrightIdeasSoftware.OLVColumn olvColumn12;
        private BrightIdeasSoftware.OLVColumn olvColumn20;
        private BrightIdeasSoftware.OLVColumn olvColumn21;
    }
}