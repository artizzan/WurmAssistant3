namespace AldursLab.WurmAssistant3.Areas.Granger
{
    partial class UCGrangerTraitView
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
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnTrait = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnHas = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setViewModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumnTrait);
            this.objectListView1.AllColumns.Add(this.olvColumnHas);
            this.objectListView1.AllColumns.Add(this.olvColumnValue);
            this.objectListView1.AllowColumnReorder = true;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnTrait,
            this.olvColumnHas,
            this.olvColumnValue});
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.GridLines = true;
            this.objectListView1.HasCollapsibleGroups = false;
            this.objectListView1.HeaderWordWrap = true;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.ShowGroups = false;
            this.objectListView1.Size = new System.Drawing.Size(307, 262);
            this.objectListView1.SortGroupItemsByPrimaryColumn = false;
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.objectListView1_ColumnReordered);
            this.objectListView1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.objectListView1_ColumnWidthChanged);
            // 
            // olvColumnTrait
            // 
            this.olvColumnTrait.AspectName = "TraitAspect";
            this.olvColumnTrait.MinimumWidth = 20;
            this.olvColumnTrait.Text = "Trait";
            this.olvColumnTrait.Width = 79;
            // 
            // olvColumnHas
            // 
            this.olvColumnHas.AspectName = "HasAspect";
            this.olvColumnHas.MinimumWidth = 20;
            this.olvColumnHas.Text = "Has";
            this.olvColumnHas.Width = 43;
            // 
            // olvColumnValue
            // 
            this.olvColumnValue.AspectName = "ValueAspect";
            this.olvColumnValue.MinimumWidth = 20;
            this.olvColumnValue.Text = "Value";
            this.olvColumnValue.Width = 51;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setViewModeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 26);
            // 
            // setViewModeToolStripMenuItem
            // 
            this.setViewModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullToolStripMenuItem,
            this.compactToolStripMenuItem,
            this.shortcutToolStripMenuItem});
            this.setViewModeToolStripMenuItem.Name = "setViewModeToolStripMenuItem";
            this.setViewModeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.setViewModeToolStripMenuItem.Text = "Set View Mode";
            // 
            // fullToolStripMenuItem
            // 
            this.fullToolStripMenuItem.Name = "fullToolStripMenuItem";
            this.fullToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.fullToolStripMenuItem.Text = "Full";
            this.fullToolStripMenuItem.Click += new System.EventHandler(this.fullToolStripMenuItem_Click);
            // 
            // compactToolStripMenuItem
            // 
            this.compactToolStripMenuItem.Name = "compactToolStripMenuItem";
            this.compactToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.compactToolStripMenuItem.Text = "Compact";
            this.compactToolStripMenuItem.Click += new System.EventHandler(this.compactToolStripMenuItem_Click);
            // 
            // shortcutToolStripMenuItem
            // 
            this.shortcutToolStripMenuItem.Name = "shortcutToolStripMenuItem";
            this.shortcutToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.shortcutToolStripMenuItem.Text = "Shortcut";
            this.shortcutToolStripMenuItem.Click += new System.EventHandler(this.shortcutToolStripMenuItem_Click);
            // 
            // UCGrangerTraitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.objectListView1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UCGrangerTraitView";
            this.Size = new System.Drawing.Size(307, 262);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumnTrait;
        private BrightIdeasSoftware.OLVColumn olvColumnValue;
        private BrightIdeasSoftware.OLVColumn olvColumnHas;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setViewModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shortcutToolStripMenuItem;

    }
}
