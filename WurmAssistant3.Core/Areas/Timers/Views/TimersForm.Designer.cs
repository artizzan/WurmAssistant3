namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views
{
    partial class TimersForm
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCustomTimers = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddRemoveChars = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkedListBoxPlayers = new System.Windows.Forms.CheckedListBox();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(232, 116);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonCustomTimers
            // 
            this.buttonCustomTimers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCustomTimers.Location = new System.Drawing.Point(78, 141);
            this.buttonCustomTimers.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCustomTimers.Name = "buttonCustomTimers";
            this.buttonCustomTimers.Size = new System.Drawing.Size(72, 38);
            this.buttonCustomTimers.TabIndex = 1;
            this.buttonCustomTimers.Text = "Custom\r\nTimers";
            this.buttonCustomTimers.UseVisualStyleBackColor = true;
            this.buttonCustomTimers.Click += new System.EventHandler(this.buttonCustomTimers_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 126);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Right-click a timer to tweak the magic";
            // 
            // buttonAddRemoveChars
            // 
            this.buttonAddRemoveChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddRemoveChars.Location = new System.Drawing.Point(3, 141);
            this.buttonAddRemoveChars.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddRemoveChars.Name = "buttonAddRemoveChars";
            this.buttonAddRemoveChars.Size = new System.Drawing.Size(71, 38);
            this.buttonAddRemoveChars.TabIndex = 3;
            this.buttonAddRemoveChars.Text = "Add\r\nRemove";
            this.buttonAddRemoveChars.UseVisualStyleBackColor = true;
            this.buttonAddRemoveChars.Click += new System.EventHandler(this.buttonAddRemoveChars_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.checkedListBoxPlayers);
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(148, 138);
            this.panel1.TabIndex = 4;
            this.panel1.Visible = false;
            // 
            // checkedListBoxPlayers
            // 
            this.checkedListBoxPlayers.CheckOnClick = true;
            this.checkedListBoxPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxPlayers.FormattingEnabled = true;
            this.checkedListBoxPlayers.Location = new System.Drawing.Point(0, 0);
            this.checkedListBoxPlayers.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBoxPlayers.Name = "checkedListBoxPlayers";
            this.checkedListBoxPlayers.Size = new System.Drawing.Size(146, 136);
            this.checkedListBoxPlayers.TabIndex = 0;
            this.checkedListBoxPlayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPlayers_ItemCheck);
            this.checkedListBoxPlayers.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxPlayers_SelectedIndexChanged);
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOptions.Location = new System.Drawing.Point(155, 141);
            this.buttonOptions.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(67, 38);
            this.buttonOptions.TabIndex = 5;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonOptions);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.flowLayoutPanel1);
            this.panel2.Controls.Add(this.buttonAddRemoveChars);
            this.panel2.Controls.Add(this.buttonCustomTimers);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.MinimumSize = new System.Drawing.Size(238, 182);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(1);
            this.panel2.Size = new System.Drawing.Size(238, 182);
            this.panel2.TabIndex = 6;
            // 
            // TimersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 182);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(245, 220);
            this.Name = "TimersForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimers_FormClosing);
            this.Load += new System.EventHandler(this.FormTimers_Load);
            this.Resize += new System.EventHandler(this.FormTimers_Resize);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonCustomTimers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddRemoveChars;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox checkedListBoxPlayers;
        private System.Windows.Forms.Button buttonOptions;
        private System.Windows.Forms.Panel panel2;
    }
}