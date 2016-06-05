namespace AldursLab.WurmAssistant3.Areas.Logging.Transients
{
    partial class CombinedLogsUserControl
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
            this.logOutput = new System.Windows.Forms.TextBox();
            this.logViewButtonsFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.errorCounter = new System.Windows.Forms.TextBox();
            this.showAppLog = new System.Windows.Forms.Button();
            this.shopAppLogDetailed = new System.Windows.Forms.Button();
            this.reportBug = new System.Windows.Forms.Button();
            this.openLogsFolderBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.logViewButtonsFlowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.logOutput, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.logViewButtonsFlowPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(502, 346);
            this.tableLayoutPanel1.TabIndex = 3;
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
            // logViewButtonsFlowPanel
            // 
            this.logViewButtonsFlowPanel.AutoScroll = true;
            this.logViewButtonsFlowPanel.AutoSize = true;
            this.logViewButtonsFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.logViewButtonsFlowPanel, 6);
            this.logViewButtonsFlowPanel.Controls.Add(this.errorCounter);
            this.logViewButtonsFlowPanel.Controls.Add(this.showAppLog);
            this.logViewButtonsFlowPanel.Controls.Add(this.shopAppLogDetailed);
            this.logViewButtonsFlowPanel.Controls.Add(this.reportBug);
            this.logViewButtonsFlowPanel.Controls.Add(this.openLogsFolderBtn);
            this.logViewButtonsFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logViewButtonsFlowPanel.Location = new System.Drawing.Point(0, 0);
            this.logViewButtonsFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.logViewButtonsFlowPanel.Name = "logViewButtonsFlowPanel";
            this.logViewButtonsFlowPanel.Size = new System.Drawing.Size(502, 40);
            this.logViewButtonsFlowPanel.TabIndex = 7;
            // 
            // errorCounter
            // 
            this.errorCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.showAppLog.Location = new System.Drawing.Point(103, 3);
            this.showAppLog.Name = "showAppLog";
            this.showAppLog.Size = new System.Drawing.Size(94, 34);
            this.showAppLog.TabIndex = 4;
            this.showAppLog.Text = "Show app log";
            this.showAppLog.UseVisualStyleBackColor = true;
            this.showAppLog.Click += new System.EventHandler(this.showAppLog_Click);
            // 
            // shopAppLogDetailed
            // 
            this.shopAppLogDetailed.Location = new System.Drawing.Point(203, 3);
            this.shopAppLogDetailed.Name = "shopAppLogDetailed";
            this.shopAppLogDetailed.Size = new System.Drawing.Size(94, 34);
            this.shopAppLogDetailed.TabIndex = 5;
            this.shopAppLogDetailed.Text = "Show app log\r\n(with trace)";
            this.shopAppLogDetailed.UseVisualStyleBackColor = true;
            this.shopAppLogDetailed.Click += new System.EventHandler(this.shopAppLogDetailed_Click);
            // 
            // reportBug
            // 
            this.reportBug.Location = new System.Drawing.Point(303, 3);
            this.reportBug.Name = "reportBug";
            this.reportBug.Size = new System.Drawing.Size(94, 34);
            this.reportBug.TabIndex = 6;
            this.reportBug.Text = "Report a bug";
            this.reportBug.UseVisualStyleBackColor = true;
            this.reportBug.Click += new System.EventHandler(this.reportBug_Click);
            // 
            // openLogsFolderBtn
            // 
            this.openLogsFolderBtn.Location = new System.Drawing.Point(403, 3);
            this.openLogsFolderBtn.Name = "openLogsFolderBtn";
            this.openLogsFolderBtn.Size = new System.Drawing.Size(94, 34);
            this.openLogsFolderBtn.TabIndex = 7;
            this.openLogsFolderBtn.Text = "Open logs\r\nfolder";
            this.openLogsFolderBtn.UseVisualStyleBackColor = true;
            this.openLogsFolderBtn.Click += new System.EventHandler(this.openLogsFolderBtn_Click);
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CombinedLogsUserControl";
            this.Size = new System.Drawing.Size(502, 346);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.logViewButtonsFlowPanel.ResumeLayout(false);
            this.logViewButtonsFlowPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox errorCounter;
        private System.Windows.Forms.Button showAppLog;
        private System.Windows.Forms.Button shopAppLogDetailed;
        private System.Windows.Forms.Button reportBug;
        private System.Windows.Forms.TextBox logOutput;
        private System.Windows.Forms.FlowLayoutPanel logViewButtonsFlowPanel;
        private System.Windows.Forms.Button openLogsFolderBtn;
    }
}
