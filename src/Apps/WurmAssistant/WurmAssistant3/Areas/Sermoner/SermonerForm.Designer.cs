namespace AldursLab.WurmAssistant3.Areas.Sermoner
{
    partial class SermonerForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.liveMonCharacterCbox = new System.Windows.Forms.ComboBox();
            this.startLiveSessionBtn = new System.Windows.Forms.Button();
            this.olvPreachers = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.lblCooldown = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.olvPreachers)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Game character:";
            // 
            // liveMonCharacterCbox
            // 
            this.liveMonCharacterCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.liveMonCharacterCbox.FormattingEnabled = true;
            this.liveMonCharacterCbox.Location = new System.Drawing.Point(12, 23);
            this.liveMonCharacterCbox.Name = "liveMonCharacterCbox";
            this.liveMonCharacterCbox.Size = new System.Drawing.Size(178, 21);
            this.liveMonCharacterCbox.TabIndex = 6;
            // 
            // startLiveSessionBtn
            // 
            this.startLiveSessionBtn.Location = new System.Drawing.Point(196, 21);
            this.startLiveSessionBtn.Name = "startLiveSessionBtn";
            this.startLiveSessionBtn.Size = new System.Drawing.Size(129, 23);
            this.startLiveSessionBtn.TabIndex = 8;
            this.startLiveSessionBtn.Text = "Start session";
            this.startLiveSessionBtn.UseVisualStyleBackColor = true;
            this.startLiveSessionBtn.Click += new System.EventHandler(this.startLiveSessionBtn_Click);
            // 
            // olvPreachers
            // 
            this.olvPreachers.AllColumns.Add(this.olvColumn1);
            this.olvPreachers.AllColumns.Add(this.olvColumn2);
            this.olvPreachers.AllColumns.Add(this.olvColumn3);
            this.olvPreachers.AllColumns.Add(this.olvColumn4);
            this.olvPreachers.AllColumns.Add(this.olvColumn5);
            this.olvPreachers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvPreachers.CellEditUseWholeCell = false;
            this.olvPreachers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5});
            this.olvPreachers.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPreachers.FullRowSelect = true;
            this.olvPreachers.HideSelection = false;
            this.olvPreachers.Location = new System.Drawing.Point(15, 50);
            this.olvPreachers.Name = "olvPreachers";
            this.olvPreachers.ShowGroups = false;
            this.olvPreachers.Size = new System.Drawing.Size(378, 351);
            this.olvPreachers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.olvPreachers.TabIndex = 11;
            this.olvPreachers.UseCompatibleStateImageBehavior = false;
            this.olvPreachers.UseFiltering = true;
            this.olvPreachers.UseNotifyPropertyChanged = true;
            this.olvPreachers.View = System.Windows.Forms.View.Details;
            this.olvPreachers.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.olvPreachers_FormatRow);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Text = "Name";
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "LastSermon";
            this.olvColumn2.Text = "LastSermon";
            this.olvColumn2.Width = 98;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Elapsed";
            this.olvColumn3.Text = "Elapsed";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "RemainingCooldown";
            this.olvColumn4.Text = "Cooldown";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "SermonCount";
            this.olvColumn5.Text = "Total";
            // 
            // lblCooldown
            // 
            this.lblCooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCooldown.AutoSize = true;
            this.lblCooldown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCooldown.ForeColor = System.Drawing.Color.Firebrick;
            this.lblCooldown.Location = new System.Drawing.Point(12, 404);
            this.lblCooldown.Name = "lblCooldown";
            this.lblCooldown.Size = new System.Drawing.Size(75, 13);
            this.lblCooldown.TabIndex = 12;
            this.lblCooldown.Text = "lblCooldown";
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Location = new System.Drawing.Point(263, 422);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(129, 23);
            this.btnSettings.TabIndex = 13;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // SermonerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 457);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.liveMonCharacterCbox);
            this.Controls.Add(this.lblCooldown);
            this.Controls.Add(this.olvPreachers);
            this.Controls.Add(this.startLiveSessionBtn);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(401, 39);
            this.Name = "SermonerForm";
            this.Text = "Sermoner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SermonerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.olvPreachers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox liveMonCharacterCbox;
        private System.Windows.Forms.Button startLiveSessionBtn;
        private BrightIdeasSoftware.ObjectListView olvPreachers;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.Label lblCooldown;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Timer timer;
    }
}