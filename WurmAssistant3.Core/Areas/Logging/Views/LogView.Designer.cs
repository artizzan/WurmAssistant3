namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Views
{
    partial class LogView
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
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sendFeedback = new System.Windows.Forms.Button();
            this.reportBug = new System.Windows.Forms.Button();
            this.shopAppLogDetailed = new System.Windows.Forms.Button();
            this.logOutput = new System.Windows.Forms.TextBox();
            this.errorCounter = new System.Windows.Forms.TextBox();
            this.showAppLog = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.sendFeedback, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.reportBug, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.shopAppLogDetailed, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.logOutput, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.errorCounter, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.showAppLog, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(502, 346);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // sendFeedback
            // 
            this.sendFeedback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sendFeedback.Location = new System.Drawing.Point(403, 3);
            this.sendFeedback.Name = "sendFeedback";
            this.sendFeedback.Size = new System.Drawing.Size(94, 34);
            this.sendFeedback.TabIndex = 7;
            this.sendFeedback.Text = "Send feedback";
            this.sendFeedback.UseVisualStyleBackColor = true;
            this.sendFeedback.Click += new System.EventHandler(this.sendFeedback_Click);
            // 
            // reportBug
            // 
            this.reportBug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportBug.Location = new System.Drawing.Point(303, 3);
            this.reportBug.Name = "reportBug";
            this.reportBug.Size = new System.Drawing.Size(94, 34);
            this.reportBug.TabIndex = 6;
            this.reportBug.Text = "Report a bug";
            this.reportBug.UseVisualStyleBackColor = true;
            this.reportBug.Click += new System.EventHandler(this.reportBug_Click);
            // 
            // shopAppLogDetailed
            // 
            this.shopAppLogDetailed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shopAppLogDetailed.Location = new System.Drawing.Point(203, 3);
            this.shopAppLogDetailed.Name = "shopAppLogDetailed";
            this.shopAppLogDetailed.Size = new System.Drawing.Size(94, 34);
            this.shopAppLogDetailed.TabIndex = 5;
            this.shopAppLogDetailed.Text = "Show app log\r\n(with trace)";
            this.shopAppLogDetailed.UseVisualStyleBackColor = true;
            this.shopAppLogDetailed.Click += new System.EventHandler(this.shopAppLogDetailed_Click);
            // 
            // logOutput
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.logOutput, 6);
            this.logOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logOutput.Location = new System.Drawing.Point(3, 43);
            this.logOutput.Multiline = true;
            this.logOutput.Name = "logOutput";
            this.logOutput.ReadOnly = true;
            this.logOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logOutput.Size = new System.Drawing.Size(496, 300);
            this.logOutput.TabIndex = 2;
            this.logOutput.TabStop = false;
            // 
            // errorCounter
            // 
            this.errorCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.errorCounter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorCounter.Location = new System.Drawing.Point(3, 3);
            this.errorCounter.Multiline = true;
            this.errorCounter.Name = "errorCounter";
            this.errorCounter.ReadOnly = true;
            this.errorCounter.Size = new System.Drawing.Size(94, 34);
            this.errorCounter.TabIndex = 3;
            this.errorCounter.TabStop = false;
            this.errorCounter.Text = "Error counter";
            this.errorCounter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // showAppLog
            // 
            this.showAppLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showAppLog.Location = new System.Drawing.Point(103, 3);
            this.showAppLog.Name = "showAppLog";
            this.showAppLog.Size = new System.Drawing.Size(94, 34);
            this.showAppLog.TabIndex = 4;
            this.showAppLog.Text = "Show app log";
            this.showAppLog.UseVisualStyleBackColor = true;
            this.showAppLog.Click += new System.EventHandler(this.showAppLog_Click);
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(502, 346);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox errorCounter;
        private System.Windows.Forms.Button showAppLog;
        private System.Windows.Forms.Button shopAppLogDetailed;
        private System.Windows.Forms.Button sendFeedback;
        private System.Windows.Forms.Button reportBug;
        private System.Windows.Forms.TextBox logOutput;
    }
}
