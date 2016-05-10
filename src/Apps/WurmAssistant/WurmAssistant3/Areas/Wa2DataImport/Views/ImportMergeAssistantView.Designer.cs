namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport.Views
{
    partial class ImportMergeAssistantView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportMergeAssistantView));
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ImportAsNewColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.DoNotImportColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ImportAllBtn = new System.Windows.Forms.Button();
            this.SkipAllExistingBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonContinue = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn1);
            this.objectListView1.AllColumns.Add(this.olvColumn2);
            this.objectListView1.AllColumns.Add(this.olvColumn3);
            this.objectListView1.AllColumns.Add(this.ImportAsNewColumn);
            this.objectListView1.AllColumns.Add(this.DoNotImportColumn);
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.ImportAsNewColumn,
            this.DoNotImportColumn});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.SelectedBackColor = System.Drawing.Color.Empty;
            this.objectListView1.SelectedForeColor = System.Drawing.Color.Empty;
            this.objectListView1.Location = new System.Drawing.Point(12, 12);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.RowHeight = 40;
            this.objectListView1.ShowGroups = false;
            this.objectListView1.ShowImagesOnSubItems = true;
            this.objectListView1.Size = new System.Drawing.Size(1260, 432);
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseSubItemCheckBoxes = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.ButtonClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.objectListView1_ButtonClick);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "SourceAspect";
            this.olvColumn1.Text = "Item to import";
            this.olvColumn1.Width = 350;
            this.olvColumn1.WordWrap = true;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "DestinationAspect";
            this.olvColumn2.Text = "Matched existing item";
            this.olvColumn2.Width = 350;
            this.olvColumn2.WordWrap = true;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "CommentAspect";
            this.olvColumn3.Text = "Comment";
            this.olvColumn3.Width = 350;
            this.olvColumn3.WordWrap = true;
            // 
            // ImportAsNewColumn
            // 
            this.ImportAsNewColumn.AspectName = "ImportAsNewAspect";
            this.ImportAsNewColumn.IsButton = true;
            this.ImportAsNewColumn.Text = "";
            this.ImportAsNewColumn.Width = 90;
            // 
            // DoNotImportColumn
            // 
            this.DoNotImportColumn.AspectName = "DoNotImportAspect";
            this.DoNotImportColumn.IsButton = true;
            this.DoNotImportColumn.Text = "";
            this.DoNotImportColumn.Width = 93;
            // 
            // ImportAllBtn
            // 
            this.ImportAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportAllBtn.Location = new System.Drawing.Point(12, 460);
            this.ImportAllBtn.Name = "ImportAllBtn";
            this.ImportAllBtn.Size = new System.Drawing.Size(128, 41);
            this.ImportAllBtn.TabIndex = 1;
            this.ImportAllBtn.Text = "Import everything\r\nnot matched";
            this.ImportAllBtn.UseVisualStyleBackColor = true;
            this.ImportAllBtn.Click += new System.EventHandler(this.ImportAllBtn_Click);
            // 
            // SkipAllExistingBtn
            // 
            this.SkipAllExistingBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SkipAllExistingBtn.Location = new System.Drawing.Point(146, 460);
            this.SkipAllExistingBtn.Name = "SkipAllExistingBtn";
            this.SkipAllExistingBtn.Size = new System.Drawing.Size(128, 41);
            this.SkipAllExistingBtn.TabIndex = 2;
            this.SkipAllExistingBtn.Text = "Clear all matched";
            this.SkipAllExistingBtn.UseVisualStyleBackColor = true;
            this.SkipAllExistingBtn.Click += new System.EventHandler(this.SkipAllExistingBtn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 452);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(398, 52);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonContinue.Location = new System.Drawing.Point(1048, 460);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(224, 41);
            this.buttonContinue.TabIndex = 4;
            this.buttonContinue.Text = "Skip the rest and continue to next batch...";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // ImportMergeAssistantView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 513);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SkipAllExistingBtn);
            this.Controls.Add(this.ImportAllBtn);
            this.Controls.Add(this.objectListView1);
            this.Name = "ImportMergeAssistantView";
            this.Text = "ImportMergeAssistantView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportMergeAssistantView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn ImportAsNewColumn;
        private BrightIdeasSoftware.OLVColumn DoNotImportColumn;
        private System.Windows.Forms.Button ImportAllBtn;
        private System.Windows.Forms.Button SkipAllExistingBtn;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Button buttonContinue;
    }
}