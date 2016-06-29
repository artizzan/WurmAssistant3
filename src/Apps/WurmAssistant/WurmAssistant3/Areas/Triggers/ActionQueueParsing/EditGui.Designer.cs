namespace AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing
{
    partial class EditGui
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.restoreDefaultsButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ConditionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Default = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Disabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Pattern = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConditionKind = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MatchingKind = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.helpBtn = new System.Windows.Forms.Button();
            this.toTxtBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ConditionId,
            this.Default,
            this.Disabled,
            this.Pattern,
            this.ConditionKind,
            this.MatchingKind});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1043, 570);
            this.dataGridView1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1049, 616);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.Controls.Add(this.saveButton, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.restoreDefaultsButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.helpBtn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.toTxtBtn, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 579);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1043, 34);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveButton.Location = new System.Drawing.Point(976, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(64, 28);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // restoreDefaultsButton
            // 
            this.restoreDefaultsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.restoreDefaultsButton.Location = new System.Drawing.Point(3, 3);
            this.restoreDefaultsButton.Name = "restoreDefaultsButton";
            this.restoreDefaultsButton.Size = new System.Drawing.Size(174, 28);
            this.restoreDefaultsButton.TabIndex = 3;
            this.restoreDefaultsButton.Text = "Restore default conditions";
            this.restoreDefaultsButton.UseVisualStyleBackColor = true;
            this.restoreDefaultsButton.Click += new System.EventHandler(this.restoreDefaultsButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(183, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(577, 34);
            this.label1.TabIndex = 4;
            this.label1.Text = "note: disable default conditions, instead of deleting\r\n(if deleted, will be resto" +
    "red on next app start)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConditionId
            // 
            this.ConditionId.DataPropertyName = "ConditionId";
            this.ConditionId.HeaderText = "ConditionId";
            this.ConditionId.Name = "ConditionId";
            this.ConditionId.ReadOnly = true;
            // 
            // Default
            // 
            this.Default.DataPropertyName = "Default";
            this.Default.HeaderText = "Default";
            this.Default.Name = "Default";
            this.Default.ReadOnly = true;
            // 
            // Disabled
            // 
            this.Disabled.DataPropertyName = "Disabled";
            this.Disabled.HeaderText = "Disabled";
            this.Disabled.Name = "Disabled";
            // 
            // Pattern
            // 
            this.Pattern.DataPropertyName = "Pattern";
            this.Pattern.HeaderText = "Pattern";
            this.Pattern.Name = "Pattern";
            // 
            // ConditionKind
            // 
            this.ConditionKind.DataPropertyName = "ConditionKind";
            this.ConditionKind.HeaderText = "Condition Type";
            this.ConditionKind.Name = "ConditionKind";
            // 
            // MatchingKind
            // 
            this.MatchingKind.DataPropertyName = "MatchingKind";
            this.MatchingKind.HeaderText = "String Matching Kind";
            this.MatchingKind.Name = "MatchingKind";
            // 
            // helpBtn
            // 
            this.helpBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpBtn.Location = new System.Drawing.Point(836, 3);
            this.helpBtn.Name = "helpBtn";
            this.helpBtn.Size = new System.Drawing.Size(64, 28);
            this.helpBtn.TabIndex = 5;
            this.helpBtn.Text = "Help";
            this.helpBtn.UseVisualStyleBackColor = true;
            this.helpBtn.Click += new System.EventHandler(this.helpBtn_Click);
            // 
            // toTxtBtn
            // 
            this.toTxtBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toTxtBtn.Location = new System.Drawing.Point(766, 3);
            this.toTxtBtn.Name = "toTxtBtn";
            this.toTxtBtn.Size = new System.Drawing.Size(64, 28);
            this.toTxtBtn.TabIndex = 6;
            this.toTxtBtn.Text = "To Txt";
            this.toTxtBtn.UseVisualStyleBackColor = true;
            this.toTxtBtn.Click += new System.EventHandler(this.toTxtBtn_Click);
            // 
            // EditGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 616);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EditGui";
            this.Text = "EditGui";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button restoreDefaultsButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConditionId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Default;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Disabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pattern;
        private System.Windows.Forms.DataGridViewComboBoxColumn ConditionKind;
        private System.Windows.Forms.DataGridViewComboBoxColumn MatchingKind;
        private System.Windows.Forms.Button helpBtn;
        private System.Windows.Forms.Button toTxtBtn;
    }
}