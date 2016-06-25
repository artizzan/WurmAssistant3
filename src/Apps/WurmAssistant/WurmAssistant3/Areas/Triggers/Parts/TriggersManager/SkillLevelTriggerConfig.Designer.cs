namespace AldursLab.WurmAssistant3.Areas.Triggers.Parts.TriggersManager
{
    partial class SkillLevelTriggerConfig
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
            this.SkillNameTbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SkillLevelInput = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.skillFeedbackLbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SkillLevelInput)).BeginInit();
            this.SuspendLayout();
            // 
            // SkillNameTbox
            // 
            this.SkillNameTbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkillNameTbox.Location = new System.Drawing.Point(93, 3);
            this.SkillNameTbox.Name = "SkillNameTbox";
            this.SkillNameTbox.Size = new System.Drawing.Size(224, 20);
            this.SkillNameTbox.TabIndex = 0;
            this.SkillNameTbox.TextChanged += new System.EventHandler(this.SkillNameTbox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Skill name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.SkillLevelInput, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SkillNameTbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.skillFeedbackLbl, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 134);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // SkillLevelInput
            // 
            this.SkillLevelInput.DecimalPlaces = 5;
            this.SkillLevelInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.SkillLevelInput.Location = new System.Drawing.Point(93, 27);
            this.SkillLevelInput.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SkillLevelInput.Name = "SkillLevelInput";
            this.SkillLevelInput.Size = new System.Drawing.Size(82, 20);
            this.SkillLevelInput.TabIndex = 4;
            this.SkillLevelInput.ValueChanged += new System.EventHandler(this.NotificationDelayInput_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Skill level:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(206, 39);
            this.label3.TabIndex = 5;
            this.label3.Text = "Triggers, when skill increases above level.\r\nSkill name is case insensitive.\r\nAls" +
    "o works for favor.";
            // 
            // skillFeedbackLbl
            // 
            this.skillFeedbackLbl.AutoSize = true;
            this.skillFeedbackLbl.Location = new System.Drawing.Point(93, 93);
            this.skillFeedbackLbl.Name = "skillFeedbackLbl";
            this.skillFeedbackLbl.Size = new System.Drawing.Size(49, 13);
            this.skillFeedbackLbl.TabIndex = 6;
            this.skillFeedbackLbl.Text = "(no data)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(0, 93);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 41);
            this.label4.TabIndex = 7;
            this.label4.Text = "Feedback:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SkillLevelTriggerConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SkillLevelTriggerConfig";
            this.Size = new System.Drawing.Size(320, 134);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SkillLevelInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox SkillNameTbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SkillLevelInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label skillFeedbackLbl;
        private System.Windows.Forms.Label label4;
    }
}
