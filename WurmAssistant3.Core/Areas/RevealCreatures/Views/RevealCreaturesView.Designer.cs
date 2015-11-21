namespace AldursLab.WurmAssistant3.Core.Areas.RevealCreatures.Views
{
    partial class RevealCreaturesView
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
            this.resultsView = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnCreature = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDirection = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDistance = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.findLatest = new System.Windows.Forms.Button();
            this.gameChar = new System.Windows.Forms.ComboBox();
            this.findText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultsView)).BeginInit();
            this.SuspendLayout();
            // 
            // resultsView
            // 
            this.resultsView.AllColumns.Add(this.olvColumnCreature);
            this.resultsView.AllColumns.Add(this.olvColumnDirection);
            this.resultsView.AllColumns.Add(this.olvColumnDistance);
            this.resultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsView.CellEditUseWholeCell = false;
            this.resultsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnCreature,
            this.olvColumnDirection,
            this.olvColumnDistance});
            this.resultsView.Cursor = System.Windows.Forms.Cursors.Default;
            this.resultsView.EmptyListMsg = "None";
            this.resultsView.HideSelection = false;
            this.resultsView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.resultsView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.resultsView.Location = new System.Drawing.Point(12, 87);
            this.resultsView.Name = "resultsView";
            this.resultsView.ShowCommandMenuOnRightClick = true;
            this.resultsView.ShowItemCountOnGroups = true;
            this.resultsView.Size = new System.Drawing.Size(486, 281);
            this.resultsView.TabIndex = 0;
            this.resultsView.UseCompatibleStateImageBehavior = false;
            this.resultsView.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnCreature
            // 
            this.olvColumnCreature.AspectName = "Creature";
            this.olvColumnCreature.Groupable = false;
            this.olvColumnCreature.MinimumWidth = 10;
            this.olvColumnCreature.Text = "Creature";
            this.olvColumnCreature.Width = 250;
            // 
            // olvColumnDirection
            // 
            this.olvColumnDirection.AspectName = "Direction";
            this.olvColumnDirection.MinimumWidth = 10;
            this.olvColumnDirection.Text = "Direction";
            this.olvColumnDirection.Width = 100;
            // 
            // olvColumnDistance
            // 
            this.olvColumnDistance.AspectName = "Distance";
            this.olvColumnDistance.MinimumWidth = 10;
            this.olvColumnDistance.Text = "Distance";
            this.olvColumnDistance.Width = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game character:";
            // 
            // findLatest
            // 
            this.findLatest.Location = new System.Drawing.Point(198, 25);
            this.findLatest.Name = "findLatest";
            this.findLatest.Size = new System.Drawing.Size(126, 21);
            this.findLatest.TabIndex = 2;
            this.findLatest.Text = "Parse latest cast";
            this.findLatest.UseVisualStyleBackColor = true;
            this.findLatest.Click += new System.EventHandler(this.findLatest_Click);
            // 
            // gameChar
            // 
            this.gameChar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gameChar.FormattingEnabled = true;
            this.gameChar.Location = new System.Drawing.Point(12, 25);
            this.gameChar.Name = "gameChar";
            this.gameChar.Size = new System.Drawing.Size(180, 21);
            this.gameChar.TabIndex = 3;
            // 
            // findText
            // 
            this.findText.Location = new System.Drawing.Point(62, 54);
            this.findText.Name = "findText";
            this.findText.Size = new System.Drawing.Size(262, 20);
            this.findText.TabIndex = 4;
            this.findText.TextChanged += new System.EventHandler(this.findText_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Search:";
            // 
            // RevealCreaturesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 380);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.findText);
            this.Controls.Add(this.gameChar);
            this.Controls.Add(this.findLatest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.resultsView);
            this.Name = "RevealCreaturesView";
            this.ShowIcon = false;
            this.Text = "Reveal Them Creatures!";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RevealCreaturesView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.resultsView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView resultsView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button findLatest;
        private System.Windows.Forms.ComboBox gameChar;
        private BrightIdeasSoftware.OLVColumn olvColumnCreature;
        private BrightIdeasSoftware.OLVColumn olvColumnDirection;
        private BrightIdeasSoftware.OLVColumn olvColumnDistance;
        private System.Windows.Forms.TextBox findText;
        private System.Windows.Forms.Label label2;
    }
}