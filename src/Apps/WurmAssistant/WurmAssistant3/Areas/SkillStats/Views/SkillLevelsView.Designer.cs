namespace AldursLab.WurmAssistant3.Areas.SkillStats.Views
{
    partial class SkillLevelsView
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
            this.descLbl = new System.Windows.Forms.Label();
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.SuspendLayout();
            // 
            // descLbl
            // 
            this.descLbl.AutoSize = true;
            this.descLbl.Location = new System.Drawing.Point(12, 9);
            this.descLbl.Name = "descLbl";
            this.descLbl.Size = new System.Drawing.Size(58, 13);
            this.descLbl.TabIndex = 2;
            this.descLbl.Text = "description";
            // 
            // objectListView
            // 
            this.objectListView.AllColumns.Add(this.olvColumn1);
            this.objectListView.AllColumns.Add(this.olvColumn2);
            this.objectListView.AllColumns.Add(this.olvColumn3);
            this.objectListView.AllowColumnReorder = true;
            this.objectListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView.CellEditUseWholeCell = false;
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.FullRowSelect = true;
            this.objectListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.objectListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.objectListView.Location = new System.Drawing.Point(12, 37);
            this.objectListView.Name = "objectListView";
            this.objectListView.ShowCommandMenuOnRightClick = true;
            this.objectListView.Size = new System.Drawing.Size(583, 405);
            this.objectListView.SortGroupItemsByPrimaryColumn = false;
            this.objectListView.TabIndex = 7;
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
            this.olvColumn3.AspectName = "CurrentValue";
            this.olvColumn3.Groupable = false;
            this.olvColumn3.Text = "Current level";
            this.olvColumn3.Width = 80;
            // 
            // SkillLevelsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 454);
            this.Controls.Add(this.objectListView);
            this.Controls.Add(this.descLbl);
            this.Name = "SkillLevelsView";
            this.ShowIcon = false;
            this.Text = "Skill levels report";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label descLbl;
        private BrightIdeasSoftware.ObjectListView objectListView;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
    }
}