namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    partial class EditTrigger
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
            this.NotificationsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SettingsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.AddNotificationButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotificationsLayout
            // 
            this.NotificationsLayout.AutoScroll = true;
            this.NotificationsLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.NotificationsLayout, 2);
            this.NotificationsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NotificationsLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.NotificationsLayout.Location = new System.Drawing.Point(3, 23);
            this.NotificationsLayout.Name = "NotificationsLayout";
            this.NotificationsLayout.Size = new System.Drawing.Size(344, 546);
            this.NotificationsLayout.TabIndex = 0;
            this.NotificationsLayout.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(178, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Notifications";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(353, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(495, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Settings";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsLayout
            // 
            this.SettingsLayout.AutoScroll = true;
            this.SettingsLayout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.SettingsLayout, 2);
            this.SettingsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SettingsLayout.Location = new System.Drawing.Point(353, 23);
            this.SettingsLayout.Name = "SettingsLayout";
            this.SettingsLayout.Size = new System.Drawing.Size(495, 546);
            this.SettingsLayout.TabIndex = 3;
            this.SettingsLayout.WrapContents = false;
            this.SettingsLayout.Layout += new System.Windows.Forms.LayoutEventHandler(this.SettingsLayout_Layout);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.NotificationsLayout, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.SettingsLayout, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.AddNotificationButton, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(851, 572);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // AddNotificationButton
            // 
            this.AddNotificationButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddNotificationButton.Location = new System.Drawing.Point(0, 0);
            this.AddNotificationButton.Margin = new System.Windows.Forms.Padding(0);
            this.AddNotificationButton.Name = "AddNotificationButton";
            this.AddNotificationButton.Size = new System.Drawing.Size(175, 20);
            this.AddNotificationButton.TabIndex = 4;
            this.AddNotificationButton.Text = "Add";
            this.AddNotificationButton.UseVisualStyleBackColor = true;
            this.AddNotificationButton.Click += new System.EventHandler(this.AddNotificationButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(861, 582);
            this.panel1.TabIndex = 5;
            // 
            // EditTrigger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 582);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTrigger";
            this.ShowIcon = false;
            this.Text = "Edit Trigger";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel NotificationsLayout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel SettingsLayout;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddNotificationButton;
    }
}