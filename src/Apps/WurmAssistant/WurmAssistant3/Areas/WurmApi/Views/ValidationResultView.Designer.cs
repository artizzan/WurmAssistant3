namespace AldursLab.WurmAssistant3.Areas.WurmApi.Views
{
    partial class ValidationResultView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationResultView));
            this.IssuesTb = new System.Windows.Forms.TextBox();
            this.StaticTb = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SkipCheckOnStart = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // IssuesTb
            // 
            this.IssuesTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IssuesTb.Location = new System.Drawing.Point(4, 144);
            this.IssuesTb.Margin = new System.Windows.Forms.Padding(4);
            this.IssuesTb.Multiline = true;
            this.IssuesTb.Name = "IssuesTb";
            this.IssuesTb.ReadOnly = true;
            this.IssuesTb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.IssuesTb.Size = new System.Drawing.Size(711, 206);
            this.IssuesTb.TabIndex = 0;
            this.IssuesTb.TabStop = false;
            // 
            // StaticTb
            // 
            this.StaticTb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StaticTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StaticTb.Location = new System.Drawing.Point(4, 4);
            this.StaticTb.Margin = new System.Windows.Forms.Padding(4);
            this.StaticTb.Multiline = true;
            this.StaticTb.Name = "StaticTb";
            this.StaticTb.ReadOnly = true;
            this.StaticTb.Size = new System.Drawing.Size(711, 132);
            this.StaticTb.TabIndex = 1;
            this.StaticTb.TabStop = false;
            this.StaticTb.Text = resources.GetString("StaticTb.Text");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.StaticTb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.IssuesTb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SkipCheckOnStart, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(719, 384);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // SkipCheckOnStart
            // 
            this.SkipCheckOnStart.AutoSize = true;
            this.SkipCheckOnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SkipCheckOnStart.Location = new System.Drawing.Point(3, 357);
            this.SkipCheckOnStart.Name = "SkipCheckOnStart";
            this.SkipCheckOnStart.Size = new System.Drawing.Size(713, 24);
            this.SkipCheckOnStart.TabIndex = 2;
            this.SkipCheckOnStart.TabStop = false;
            this.SkipCheckOnStart.Text = "Skip this check on Wurm Assistant startup. (Can be done manually from Options men" +
    "u)";
            this.SkipCheckOnStart.UseVisualStyleBackColor = true;
            this.SkipCheckOnStart.CheckedChanged += new System.EventHandler(this.SkipCheckOnStart_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(729, 394);
            this.panel1.TabIndex = 3;
            // 
            // ValidationResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 394);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ValidationResultView";
            this.ShowIcon = false;
            this.Text = "WurmApi detected potential issues";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox IssuesTb;
        private System.Windows.Forms.TextBox StaticTb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox SkipCheckOnStart;
        private System.Windows.Forms.Panel panel1;
    }
}