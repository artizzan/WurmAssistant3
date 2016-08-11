namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    partial class SimpleConditionTriggerBaseConfig
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
            this.ConditionTbox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sourceTbox = new System.Windows.Forms.TextBox();
            this.checkBoxMatchEveryLine = new System.Windows.Forms.CheckBox();
            this.SourceHelpLabel = new System.Windows.Forms.Label();
            this.DescLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConditionTbox
            // 
            this.ConditionTbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConditionTbox.Location = new System.Drawing.Point(93, 3);
            this.ConditionTbox.Name = "ConditionTbox";
            this.ConditionTbox.Size = new System.Drawing.Size(224, 20);
            this.ConditionTbox.TabIndex = 0;
            this.ConditionTbox.TextChanged += new System.EventHandler(this.ConditionTbox_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ConditionTbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.sourceTbox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxMatchEveryLine, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.SourceHelpLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.DescLabel, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 190);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log entry content:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 26);
            this.label2.TabIndex = 5;
            this.label2.Text = "Log entry source:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sourceTbox
            // 
            this.sourceTbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceTbox.Location = new System.Drawing.Point(93, 72);
            this.sourceTbox.Name = "sourceTbox";
            this.sourceTbox.Size = new System.Drawing.Size(224, 20);
            this.sourceTbox.TabIndex = 6;
            this.sourceTbox.TextChanged += new System.EventHandler(this.sourceTbox_TextChanged);
            // 
            // checkBoxMatchEveryLine
            // 
            this.checkBoxMatchEveryLine.AutoSize = true;
            this.checkBoxMatchEveryLine.Location = new System.Drawing.Point(93, 42);
            this.checkBoxMatchEveryLine.Name = "checkBoxMatchEveryLine";
            this.checkBoxMatchEveryLine.Size = new System.Drawing.Size(118, 17);
            this.checkBoxMatchEveryLine.TabIndex = 4;
            this.checkBoxMatchEveryLine.Text = "Match any content.";
            this.checkBoxMatchEveryLine.UseVisualStyleBackColor = true;
            this.checkBoxMatchEveryLine.CheckedChanged += new System.EventHandler(this.checkBoxMatchEveryLine_CheckedChanged);
            // 
            // SourceHelpLabel
            // 
            this.SourceHelpLabel.AutoSize = true;
            this.SourceHelpLabel.Location = new System.Drawing.Point(93, 95);
            this.SourceHelpLabel.Name = "SourceHelpLabel";
            this.SourceHelpLabel.Size = new System.Drawing.Size(106, 13);
            this.SourceHelpLabel.TabIndex = 7;
            this.SourceHelpLabel.Text = "source help text here";
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.Location = new System.Drawing.Point(93, 26);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(110, 13);
            this.DescLabel.TabIndex = 2;
            this.DescLabel.Text = "content help text here";
            // 
            // SimpleConditionTriggerBaseConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SimpleConditionTriggerBaseConfig";
            this.Size = new System.Drawing.Size(320, 190);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ConditionTbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DescLabel;
        private System.Windows.Forms.CheckBox checkBoxMatchEveryLine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sourceTbox;
        private System.Windows.Forms.Label SourceHelpLabel;
    }
}
