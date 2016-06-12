namespace AldursLab.WurmAssistant3.Areas.Core.Services
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MenuViewPanel = new System.Windows.Forms.Panel();
            this.LogViewPanel = new System.Windows.Forms.Panel();
            this.featuresFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.MenuViewPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LogViewPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.featuresFlowPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 413);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // MenuViewPanel
            // 
            this.MenuViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuViewPanel.Location = new System.Drawing.Point(3, 3);
            this.MenuViewPanel.Name = "MenuViewPanel";
            this.MenuViewPanel.Size = new System.Drawing.Size(600, 24);
            this.MenuViewPanel.TabIndex = 0;
            // 
            // LogViewPanel
            // 
            this.LogViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogViewPanel.Location = new System.Drawing.Point(3, 39);
            this.LogViewPanel.Name = "LogViewPanel";
            this.LogViewPanel.Size = new System.Drawing.Size(600, 371);
            this.LogViewPanel.TabIndex = 2;
            // 
            // featuresFlowPanel
            // 
            this.featuresFlowPanel.AutoScroll = true;
            this.featuresFlowPanel.AutoSize = true;
            this.featuresFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.featuresFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.featuresFlowPanel.Location = new System.Drawing.Point(3, 33);
            this.featuresFlowPanel.Name = "featuresFlowPanel";
            this.featuresFlowPanel.Size = new System.Drawing.Size(600, 1);
            this.featuresFlowPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 413);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Window tilte is set in code";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel MenuViewPanel;
        private System.Windows.Forms.Panel LogViewPanel;
        private System.Windows.Forms.FlowLayoutPanel featuresFlowPanel;
        private System.Windows.Forms.ToolTip toolTips;
    }
}