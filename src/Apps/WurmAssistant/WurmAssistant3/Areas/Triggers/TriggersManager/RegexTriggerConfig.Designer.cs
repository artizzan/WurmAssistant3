using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    partial class RegexTriggerConfig
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.WhatsRegularExprLink = new System.Windows.Forms.LinkLabel();
            this.RegexGuideLink = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelAutowrap1 = new AldursLab.WurmAssistant3.Utils.WinForms.LabelAutowrap();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.WhatsRegularExprLink, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.RegexGuideLink, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelAutowrap1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 162);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(0, 105);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "More help:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WhatsRegularExprLink
            // 
            this.WhatsRegularExprLink.AutoSize = true;
            this.WhatsRegularExprLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WhatsRegularExprLink.Location = new System.Drawing.Point(93, 105);
            this.WhatsRegularExprLink.Name = "WhatsRegularExprLink";
            this.WhatsRegularExprLink.Size = new System.Drawing.Size(224, 25);
            this.WhatsRegularExprLink.TabIndex = 5;
            this.WhatsRegularExprLink.TabStop = true;
            this.WhatsRegularExprLink.Text = "What is a regular expression?";
            this.WhatsRegularExprLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.WhatsRegularExprLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WhatsRegularExprLink_LinkClicked);
            // 
            // RegexGuideLink
            // 
            this.RegexGuideLink.AutoSize = true;
            this.RegexGuideLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegexGuideLink.Location = new System.Drawing.Point(93, 130);
            this.RegexGuideLink.Name = "RegexGuideLink";
            this.RegexGuideLink.Size = new System.Drawing.Size(224, 32);
            this.RegexGuideLink.TabIndex = 6;
            this.RegexGuideLink.TabStop = true;
            this.RegexGuideLink.Text = "30 minute regex guide";
            this.RegexGuideLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RegexGuideLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RegexGuideLink_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 80);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hint:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAutowrap1.Location = new System.Drawing.Point(93, 0);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(224, 78);
            this.labelAutowrap1.TabIndex = 2;
            this.labelAutowrap1.Text = "do not include timestamp in the pattern, it is stripped already\r\nthis will not wo" +
    "rk: \r\n\\[\\d\\d\\:\\d\\d\\:\\d\\d\\] You see a wall\r\nthis will: \r\nYou see a wall";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Testing:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(93, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(224, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "if it works in Log Searcher, it will work here";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RegexTriggerConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RegexTriggerConfig";
            this.Size = new System.Drawing.Size(320, 162);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel WhatsRegularExprLink;
        private System.Windows.Forms.LinkLabel RegexGuideLink;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}
