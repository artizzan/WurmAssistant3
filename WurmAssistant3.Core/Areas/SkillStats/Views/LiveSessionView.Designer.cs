namespace AldursLab.WurmAssistant3.Core.Areas.SkillStats.Views
{
    partial class LiveSessionView
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
            this.startPauseBtn = new System.Windows.Forms.Button();
            this.statusLbl = new System.Windows.Forms.Label();
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.SuspendLayout();
            // 
            // startPauseBtn
            // 
            this.startPauseBtn.Location = new System.Drawing.Point(12, 12);
            this.startPauseBtn.Name = "startPauseBtn";
            this.startPauseBtn.Size = new System.Drawing.Size(75, 23);
            this.startPauseBtn.TabIndex = 1;
            this.startPauseBtn.Text = "Start";
            this.startPauseBtn.UseVisualStyleBackColor = true;
            this.startPauseBtn.Click += new System.EventHandler(this.startPauseBtn_Click);
            // 
            // statusLbl
            // 
            this.statusLbl.AutoSize = true;
            this.statusLbl.Location = new System.Drawing.Point(93, 17);
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(35, 13);
            this.statusLbl.TabIndex = 3;
            this.statusLbl.Text = "label1";
            // 
            // objectListView
            // 
            this.objectListView.AllColumns.Add(this.olvColumn1);
            this.objectListView.AllColumns.Add(this.olvColumn2);
            this.objectListView.AllColumns.Add(this.olvColumn3);
            this.objectListView.AllColumns.Add(this.olvColumn4);
            this.objectListView.AllColumns.Add(this.olvColumn5);
            this.objectListView.AllowColumnReorder = true;
            this.objectListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView.CellEditUseWholeCell = false;
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.FullRowSelect = true;
            this.objectListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.objectListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.objectListView.Location = new System.Drawing.Point(12, 41);
            this.objectListView.Name = "objectListView";
            this.objectListView.ShowCommandMenuOnRightClick = true;
            this.objectListView.ShowGroups = false;
            this.objectListView.Size = new System.Drawing.Size(547, 345);
            this.objectListView.TabIndex = 4;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Text = "Skill name";
            this.olvColumn1.Width = 140;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "StartValue";
            this.olvColumn2.Text = "Start level";
            this.olvColumn2.Width = 80;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "CurrentValue";
            this.olvColumn3.Text = "Current level";
            this.olvColumn3.Width = 80;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "AverageGainPerHour";
            this.olvColumn4.AspectToStringFormat = "{0:0.000}";
            this.olvColumn4.Text = "Average gain per hour";
            this.olvColumn4.Width = 140;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "TotalGain";
            this.olvColumn5.AspectToStringFormat = "{0:0.000}";
            this.olvColumn5.Text = "Total gain";
            this.olvColumn5.Width = 80;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // LiveSessionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 398);
            this.Controls.Add(this.objectListView);
            this.Controls.Add(this.statusLbl);
            this.Controls.Add(this.startPauseBtn);
            this.Name = "LiveSessionView";
            this.ShowIcon = false;
            this.Text = "Skills Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LiveSessionView_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.LiveSessionView_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startPauseBtn;
        private System.Windows.Forms.Label statusLbl;
        private BrightIdeasSoftware.ObjectListView objectListView;
        private System.Windows.Forms.Timer timer;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
    }
}