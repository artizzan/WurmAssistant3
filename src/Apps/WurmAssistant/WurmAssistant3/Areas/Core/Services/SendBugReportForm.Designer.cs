using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    partial class SendBugReportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendBugReportForm));
            this.sendMailBtn = new System.Windows.Forms.Button();
            this.sendPmBtn = new System.Windows.Forms.Button();
            this.postOnForumBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelAutowrap1 = new LabelAutowrap();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendMailBtn
            // 
            this.sendMailBtn.Location = new System.Drawing.Point(366, 147);
            this.sendMailBtn.Margin = new System.Windows.Forms.Padding(9);
            this.sendMailBtn.Name = "sendMailBtn";
            this.sendMailBtn.Size = new System.Drawing.Size(145, 27);
            this.sendMailBtn.TabIndex = 1;
            this.sendMailBtn.Text = "Send mail";
            this.sendMailBtn.UseVisualStyleBackColor = true;
            this.sendMailBtn.Click += new System.EventHandler(this.sendMailBtn_Click);
            // 
            // sendPmBtn
            // 
            this.sendPmBtn.Location = new System.Drawing.Point(187, 147);
            this.sendPmBtn.Margin = new System.Windows.Forms.Padding(9);
            this.sendPmBtn.Name = "sendPmBtn";
            this.sendPmBtn.Size = new System.Drawing.Size(145, 27);
            this.sendPmBtn.TabIndex = 2;
            this.sendPmBtn.Text = "Send PM";
            this.sendPmBtn.UseVisualStyleBackColor = true;
            this.sendPmBtn.Click += new System.EventHandler(this.sendPmBtn_Click);
            // 
            // postOnForumBtn
            // 
            this.postOnForumBtn.Location = new System.Drawing.Point(9, 147);
            this.postOnForumBtn.Margin = new System.Windows.Forms.Padding(9);
            this.postOnForumBtn.Name = "postOnForumBtn";
            this.postOnForumBtn.Size = new System.Drawing.Size(145, 27);
            this.postOnForumBtn.TabIndex = 3;
            this.postOnForumBtn.Text = "Post on forum";
            this.postOnForumBtn.UseVisualStyleBackColor = true;
            this.postOnForumBtn.Click += new System.EventHandler(this.postOnForumBtn_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.postOnForumBtn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAutowrap1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sendMailBtn, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.sendPmBtn, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(537, 215);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.labelAutowrap1, 3);
            this.labelAutowrap1.Location = new System.Drawing.Point(9, 9);
            this.labelAutowrap1.Margin = new System.Windows.Forms.Padding(9);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(519, 120);
            this.labelAutowrap1.TabIndex = 0;
            this.labelAutowrap1.Text = resources.GetString("labelAutowrap1.Text");
            // 
            // SendBugReportView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 215);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SendBugReportForm";
            this.ShowIcon = false;
            this.Text = "Send bug report...";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.Button sendMailBtn;
        private System.Windows.Forms.Button sendPmBtn;
        private System.Windows.Forms.Button postOnForumBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}