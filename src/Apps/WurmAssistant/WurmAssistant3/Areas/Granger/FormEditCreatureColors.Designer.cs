namespace AldursLab.WurmAssistant3.Areas.Granger
{
    partial class FormEditCreatureColors
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
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnColor = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnChangeColor = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnRemove = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonAddNew = new System.Windows.Forms.Button();
            this.textBoxId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectListView
            // 
            this.objectListView.AllColumns.Add(this.olvColumnId);
            this.objectListView.AllColumns.Add(this.olvColumnColor);
            this.objectListView.AllColumns.Add(this.olvColumnChangeColor);
            this.objectListView.AllColumns.Add(this.olvColumnRemove);
            this.objectListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView.CellEditUseWholeCell = false;
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnId,
            this.olvColumnColor,
            this.olvColumnChangeColor,
            this.olvColumnRemove});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.GridLines = true;
            this.objectListView.HasCollapsibleGroups = false;
            this.objectListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListView.Location = new System.Drawing.Point(12, 12);
            this.objectListView.Name = "objectListView";
            this.objectListView.RowHeight = 25;
            this.objectListView.Size = new System.Drawing.Size(509, 350);
            this.objectListView.TabIndex = 0;
            this.objectListView.UseCellFormatEvents = true;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            this.objectListView.ButtonClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.objectListView_ButtonClick);
            this.objectListView.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.objectListView_FormatCell);
            // 
            // olvColumnId
            // 
            this.olvColumnId.AspectName = "Id";
            this.olvColumnId.Groupable = false;
            this.olvColumnId.Text = "Id";
            this.olvColumnId.Width = 110;
            // 
            // olvColumnColor
            // 
            this.olvColumnColor.AspectName = "Color";
            this.olvColumnColor.Groupable = false;
            this.olvColumnColor.Text = "Color";
            this.olvColumnColor.Width = 180;
            // 
            // olvColumnChangeColor
            // 
            this.olvColumnChangeColor.AspectName = "ChangeColorButtonAspect";
            this.olvColumnChangeColor.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.olvColumnChangeColor.Groupable = false;
            this.olvColumnChangeColor.IsButton = true;
            this.olvColumnChangeColor.Text = "";
            this.olvColumnChangeColor.Width = 96;
            // 
            // olvColumnRemove
            // 
            this.olvColumnRemove.AspectName = "RemoveButtonAspect";
            this.olvColumnRemove.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.olvColumnRemove.Groupable = false;
            this.olvColumnRemove.IsButton = true;
            this.olvColumnRemove.Text = "";
            this.olvColumnRemove.Width = 80;
            // 
            // buttonAddNew
            // 
            this.buttonAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddNew.Location = new System.Drawing.Point(150, 46);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new System.Drawing.Size(75, 23);
            this.buttonAddNew.TabIndex = 1;
            this.buttonAddNew.Text = "Add";
            this.buttonAddNew.UseVisualStyleBackColor = true;
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // textBoxId
            // 
            this.textBoxId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxId.Location = new System.Drawing.Point(34, 20);
            this.textBoxId.Name = "textBoxId";
            this.textBoxId.Size = new System.Drawing.Size(191, 20);
            this.textBoxId.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Id:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBoxId);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonAddNew);
            this.groupBox1.Location = new System.Drawing.Point(12, 368);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 81);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add new color";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 369);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 39);
            this.label2.TabIndex = 5;
            this.label2.Text = "Note: \r\nDisabled colors are built in and currently \r\ncan\'t be removed or changed." +
    "";
            // 
            // FormEditCreatureColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 461);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.objectListView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditCreatureColors";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editing Creature Colors";
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private BrightIdeasSoftware.ObjectListView objectListView;
        private System.Windows.Forms.Button buttonAddNew;
        private System.Windows.Forms.TextBox textBoxId;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.OLVColumn olvColumnId;
        private BrightIdeasSoftware.OLVColumn olvColumnColor;
        private BrightIdeasSoftware.OLVColumn olvColumnChangeColor;
        private BrightIdeasSoftware.OLVColumn olvColumnRemove;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
    }
}