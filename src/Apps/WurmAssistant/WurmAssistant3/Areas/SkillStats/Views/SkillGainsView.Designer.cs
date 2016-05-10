namespace AldursLab.WurmAssistant3.Areas.SkillStats.Views
{
    partial class SkillGainsView
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
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.descLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.SuspendLayout();
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
            this.objectListView.SelectedBackColor = System.Drawing.Color.Empty;
            this.objectListView.SelectedForeColor = System.Drawing.Color.Empty;
            this.objectListView.Location = new System.Drawing.Point(12, 40);
            this.objectListView.Name = "objectListView";
            this.objectListView.ShowCommandMenuOnRightClick = true;
            this.objectListView.Size = new System.Drawing.Size(577, 415);
            this.objectListView.SortGroupItemsByPrimaryColumn = false;
            this.objectListView.TabIndex = 5;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "GameCharacter";
            this.olvColumn1.Text = "Character";
            this.olvColumn1.Width = 80;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Name";
            this.olvColumn2.Groupable = false;
            this.olvColumn2.Text = "Skill name";
            this.olvColumn2.Width = 140;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "StartValue";
            this.olvColumn3.Groupable = false;
            this.olvColumn3.Text = "Start level";
            this.olvColumn3.Width = 80;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "CurrentValue";
            this.olvColumn4.Groupable = false;
            this.olvColumn4.Text = "Current level";
            this.olvColumn4.Width = 80;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Gain";
            this.olvColumn5.AspectToStringFormat = "{0:0.000000}";
            this.olvColumn5.Groupable = false;
            this.olvColumn5.Text = "Total gain";
            this.olvColumn5.Width = 80;
            // 
            // descLbl
            // 
            this.descLbl.AutoSize = true;
            this.descLbl.Location = new System.Drawing.Point(12, 9);
            this.descLbl.Name = "descLbl";
            this.descLbl.Size = new System.Drawing.Size(58, 13);
            this.descLbl.TabIndex = 6;
            this.descLbl.Text = "description";
            // 
            // SkillGainsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 467);
            this.Controls.Add(this.descLbl);
            this.Controls.Add(this.objectListView);
            this.Name = "SkillGainsView";
            this.ShowIcon = false;
            this.Text = "Skill gains report";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private System.Windows.Forms.Label descLbl;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
    }
}