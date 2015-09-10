namespace AldursLab.WurmAssistant3.Core.Areas.Root.Views
{
    partial class MainView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MenuViewPanel = new System.Windows.Forms.Panel();
            this.ModulesViewPanel = new System.Windows.Forms.Panel();
            this.LogViewPanel = new System.Windows.Forms.Panel();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.InitTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.MenuViewPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ModulesViewPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LogViewPanel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 329);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // MenuViewPanel
            // 
            this.MenuViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuViewPanel.Location = new System.Drawing.Point(3, 3);
            this.MenuViewPanel.Name = "MenuViewPanel";
            this.MenuViewPanel.Size = new System.Drawing.Size(600, 19);
            this.MenuViewPanel.TabIndex = 0;
            // 
            // ModulesViewPanel
            // 
            this.ModulesViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModulesViewPanel.Location = new System.Drawing.Point(3, 28);
            this.ModulesViewPanel.Name = "ModulesViewPanel";
            this.ModulesViewPanel.Size = new System.Drawing.Size(600, 194);
            this.ModulesViewPanel.TabIndex = 1;
            // 
            // LogViewPanel
            // 
            this.LogViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogViewPanel.Location = new System.Drawing.Point(3, 228);
            this.LogViewPanel.Name = "LogViewPanel";
            this.LogViewPanel.Size = new System.Drawing.Size(600, 98);
            this.LogViewPanel.TabIndex = 2;
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 500;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // InitTimer
            // 
            this.InitTimer.Tick += new System.EventHandler(this.InitTimer_Tick);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 329);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainView";
            this.Text = "MainView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainView_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel MenuViewPanel;
        private System.Windows.Forms.Panel ModulesViewPanel;
        private System.Windows.Forms.Panel LogViewPanel;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Timer InitTimer;
    }
}