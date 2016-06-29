namespace AldursLab.WurmAssistant3.Areas.Granger
{
    partial class UCGrangerHerdList
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
            this.contextMenuStripHerds = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addCreatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnHerd = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStripHerds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStripHerds
            // 
            this.contextMenuStripHerds.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolStripMenuItem,
            this.toolStripSeparator2,
            this.addCreatureToolStripMenuItem,
            this.toolStripSeparator3,
            this.renameToolStripMenuItem,
            this.combineToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem});
            this.contextMenuStripHerds.Name = "contextMenuStripHerds";
            this.contextMenuStripHerds.Size = new System.Drawing.Size(206, 132);
            // 
            // addNewToolStripMenuItem
            // 
            this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            this.addNewToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.addNewToolStripMenuItem.Text = "Create new herd";
            this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // addCreatureToolStripMenuItem
            // 
            this.addCreatureToolStripMenuItem.Name = "addCreatureToolStripMenuItem";
            this.addCreatureToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.addCreatureToolStripMenuItem.Text = "Add creature to this herd";
            this.addCreatureToolStripMenuItem.Click += new System.EventHandler(this.addCreatureToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(202, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // combineToolStripMenuItem
            // 
            this.combineToolStripMenuItem.Name = "combineToolStripMenuItem";
            this.combineToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.combineToolStripMenuItem.Text = "Merge";
            this.combineToolStripMenuItem.Click += new System.EventHandler(this.combineToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.deleteToolStripMenuItem.Text = "Remove";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnHerd);
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.CheckBoxes = true;
            this.objectListView1.CheckedAspectName = "CheckedAspect";
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnHerd});
            this.objectListView1.ContextMenuStrip = this.contextMenuStripHerds;
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HasCollapsibleGroups = false;
            this.objectListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListView1.SelectedBackColor = System.Drawing.Color.Empty;
            this.objectListView1.SelectedForeColor = System.Drawing.Color.Empty;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Margin = new System.Windows.Forms.Padding(2);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowGroups = false;
            this.objectListView1.Size = new System.Drawing.Size(309, 318);
            this.objectListView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.objectListView1.TabIndex = 4;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.objectListView1_CellRightClick);
            this.objectListView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.objectListView1_ItemChecked);
            // 
            // olvColumnHerd
            // 
            this.olvColumnHerd.AspectName = "HerdIDAspect";
            this.olvColumnHerd.FillsFreeSpace = true;
            this.olvColumnHerd.MinimumWidth = 20;
            this.olvColumnHerd.Text = "Active herds";
            this.olvColumnHerd.Width = 104;
            // 
            // UCGrangerHerdList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.objectListView1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCGrangerHerdList";
            this.Size = new System.Drawing.Size(309, 318);
            this.contextMenuStripHerds.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripHerds;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem combineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addCreatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnHerd;

    }
}
