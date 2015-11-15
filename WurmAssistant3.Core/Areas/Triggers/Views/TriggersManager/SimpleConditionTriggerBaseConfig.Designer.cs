namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager
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
            this.DescLabel = new System.Windows.Forms.Label();
            this.checkBoxMatchEveryLine = new System.Windows.Forms.CheckBox();
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
            this.tableLayoutPanel1.Controls.Add(this.DescLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxMatchEveryLine, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 74);
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
            this.label1.Text = "Condition:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DescLabel
            // 
            this.DescLabel.AutoSize = true;
            this.DescLabel.Location = new System.Drawing.Point(93, 52);
            this.DescLabel.Name = "DescLabel";
            this.DescLabel.Size = new System.Drawing.Size(117, 13);
            this.DescLabel.TabIndex = 2;
            this.DescLabel.Text = "condition help text here";
            // 
            // checkBoxMatchEveryLine
            // 
            this.checkBoxMatchEveryLine.AutoSize = true;
            this.checkBoxMatchEveryLine.Location = new System.Drawing.Point(93, 29);
            this.checkBoxMatchEveryLine.Name = "checkBoxMatchEveryLine";
            this.checkBoxMatchEveryLine.Size = new System.Drawing.Size(104, 17);
            this.checkBoxMatchEveryLine.TabIndex = 4;
            this.checkBoxMatchEveryLine.Text = "Match every line";
            this.checkBoxMatchEveryLine.UseVisualStyleBackColor = true;
            this.checkBoxMatchEveryLine.CheckedChanged += new System.EventHandler(this.checkBoxMatchEveryLine_CheckedChanged);
            // 
            // SimpleConditionTriggerBaseConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SimpleConditionTriggerBaseConfig";
            this.Size = new System.Drawing.Size(320, 74);
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
    }
}
