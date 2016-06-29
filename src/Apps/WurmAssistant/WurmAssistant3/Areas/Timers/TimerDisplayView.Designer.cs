namespace AldursLab.WurmAssistant3.Areas.Timers
{
    partial class TimerDisplayView
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTimeTo = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(210, 20);
            this.tableLayoutPanel1.TabIndex = 3;
            this.tableLayoutPanel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanel2.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelTimeTo, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(210, 15);
            this.tableLayoutPanel2.TabIndex = 0;
            this.tableLayoutPanel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel2_MouseClick);
            // 
            // labelTimeTo
            // 
            this.labelTimeTo.AutoSize = true;
            this.labelTimeTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTimeTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTimeTo.Location = new System.Drawing.Point(122, 0);
            this.labelTimeTo.Margin = new System.Windows.Forms.Padding(0);
            this.labelTimeTo.Name = "labelTimeTo";
            this.labelTimeTo.Size = new System.Drawing.Size(88, 15);
            this.labelTimeTo.TabIndex = 2;
            this.labelTimeTo.Text = "(crafting...)";
            this.labelTimeTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelTimeTo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelTimeTo_MouseClick);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelName.Location = new System.Drawing.Point(0, 0);
            this.labelName.Margin = new System.Windows.Forms.Padding(0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(122, 15);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "name";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelName_MouseClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 15);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(210, 5);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseClick);
            // 
            // UControlTimerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TimerDisplayView";
            this.Size = new System.Drawing.Size(210, 20);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UControlTimerDisplay_MouseClick);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelTimeTo;
        private System.Windows.Forms.ProgressBar progressBar1;


    }
}
