namespace AldursLab.WurmAssistant3.Utils.WinForms
{
    partial class TimeSpanInputCompact
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DaysTb = new System.Windows.Forms.TextBox();
            this.HoursTb = new System.Windows.Forms.TextBox();
            this.MinutesTb = new System.Windows.Forms.TextBox();
            this.SecondsTb = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(27, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 22);
            this.label1.TabIndex = 4;
            this.label1.Text = "d";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.DaysTb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.HoursTb, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.MinutesTb, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.SecondsTb, 6, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(170, 22);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(150, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 22);
            this.label4.TabIndex = 7;
            this.label4.Text = "s";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(109, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "m";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(68, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 22);
            this.label2.TabIndex = 5;
            this.label2.Text = "h";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DaysTb
            // 
            this.DaysTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DaysTb.Location = new System.Drawing.Point(1, 1);
            this.DaysTb.Margin = new System.Windows.Forms.Padding(1);
            this.DaysTb.Name = "DaysTb";
            this.DaysTb.Size = new System.Drawing.Size(23, 20);
            this.DaysTb.TabIndex = 8;
            this.DaysTb.TextChanged += new System.EventHandler(this.DaysTb_TextChanged);
            // 
            // HoursTb
            // 
            this.HoursTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HoursTb.Location = new System.Drawing.Point(42, 1);
            this.HoursTb.Margin = new System.Windows.Forms.Padding(1);
            this.HoursTb.Name = "HoursTb";
            this.HoursTb.Size = new System.Drawing.Size(23, 20);
            this.HoursTb.TabIndex = 9;
            this.HoursTb.TextChanged += new System.EventHandler(this.HoursTb_TextChanged);
            // 
            // MinutesTb
            // 
            this.MinutesTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinutesTb.Location = new System.Drawing.Point(83, 1);
            this.MinutesTb.Margin = new System.Windows.Forms.Padding(1);
            this.MinutesTb.Name = "MinutesTb";
            this.MinutesTb.Size = new System.Drawing.Size(23, 20);
            this.MinutesTb.TabIndex = 10;
            this.MinutesTb.TextChanged += new System.EventHandler(this.MinutesTb_TextChanged);
            // 
            // SecondsTb
            // 
            this.SecondsTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecondsTb.Location = new System.Drawing.Point(124, 1);
            this.SecondsTb.Margin = new System.Windows.Forms.Padding(1);
            this.SecondsTb.Name = "SecondsTb";
            this.SecondsTb.Size = new System.Drawing.Size(23, 20);
            this.SecondsTb.TabIndex = 11;
            this.SecondsTb.TextChanged += new System.EventHandler(this.SecondsTb_TextChanged);
            // 
            // TimeSpanInputCompact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TimeSpanInputCompact";
            this.Size = new System.Drawing.Size(170, 22);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DaysTb;
        private System.Windows.Forms.TextBox HoursTb;
        private System.Windows.Forms.TextBox MinutesTb;
        private System.Windows.Forms.TextBox SecondsTb;

    }
}
