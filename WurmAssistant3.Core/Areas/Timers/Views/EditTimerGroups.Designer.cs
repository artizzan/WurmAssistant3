namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views
{
    partial class EditTimerGroups
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
            this.currentGroupsLb = new System.Windows.Forms.ListBox();
            this.serverGroupCb = new System.Windows.Forms.ComboBox();
            this.playerCb = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelAutowrap1 = new AldursLab.WurmAssistant3.Core.WinForms.LabelAutowrap();
            this.addBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.removeBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.hideGroupBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // currentGroupsLb
            // 
            this.currentGroupsLb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentGroupsLb.FormattingEnabled = true;
            this.currentGroupsLb.Location = new System.Drawing.Point(3, 16);
            this.currentGroupsLb.Name = "currentGroupsLb";
            this.currentGroupsLb.Size = new System.Drawing.Size(574, 179);
            this.currentGroupsLb.TabIndex = 0;
            // 
            // serverGroupCb
            // 
            this.serverGroupCb.FormattingEnabled = true;
            this.serverGroupCb.Location = new System.Drawing.Point(9, 93);
            this.serverGroupCb.Name = "serverGroupCb";
            this.serverGroupCb.Size = new System.Drawing.Size(245, 21);
            this.serverGroupCb.TabIndex = 2;
            // 
            // playerCb
            // 
            this.playerCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.playerCb.FormattingEnabled = true;
            this.playerCb.Location = new System.Drawing.Point(9, 44);
            this.playerCb.Name = "playerCb";
            this.playerCb.Size = new System.Drawing.Size(245, 21);
            this.playerCb.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelAutowrap1);
            this.groupBox1.Controls.Add(this.addBtn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.playerCb);
            this.groupBox1.Controls.Add(this.serverGroupCb);
            this.groupBox1.Location = new System.Drawing.Point(598, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 267);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add new timers group";
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAutowrap1.Location = new System.Drawing.Point(12, 121);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(252, 78);
            this.labelAutowrap1.TabIndex = 6;
            this.labelAutowrap1.Text = "Choose one of defined server groups, or type in a \"server scoped\" group, to creat" +
    "e timers for ungrouped server.\r\n\r\nServer scoped groups have a simple format:\r\nSE" +
    "RVERSCOPE:SERVERNAME";
            // 
            // addBtn
            // 
            this.addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addBtn.Location = new System.Drawing.Point(172, 228);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(92, 33);
            this.addBtn.TabIndex = 5;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Character name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Server group:";
            // 
            // removeBtn
            // 
            this.removeBtn.Location = new System.Drawing.Point(6, 19);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(80, 38);
            this.removeBtn.TabIndex = 6;
            this.removeBtn.Text = "Remove";
            this.removeBtn.UseVisualStyleBackColor = true;
            this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.hideGroupBtn);
            this.groupBox2.Controls.Add(this.moveDownBtn);
            this.groupBox2.Controls.Add(this.moveUpBtn);
            this.groupBox2.Controls.Add(this.removeBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 216);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(577, 63);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "modify selected";
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(491, 19);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(80, 39);
            this.moveDownBtn.TabIndex = 8;
            this.moveDownBtn.Text = "Move Down";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(405, 19);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(80, 39);
            this.moveUpBtn.TabIndex = 7;
            this.moveUpBtn.Text = "Move Up";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.currentGroupsLb);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(580, 198);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current timer groups";
            // 
            // hideGroupBtn
            // 
            this.hideGroupBtn.Location = new System.Drawing.Point(116, 18);
            this.hideGroupBtn.Name = "hideGroupBtn";
            this.hideGroupBtn.Size = new System.Drawing.Size(80, 39);
            this.hideGroupBtn.TabIndex = 9;
            this.hideGroupBtn.Text = "Toggle\r\nhidden";
            this.hideGroupBtn.UseVisualStyleBackColor = true;
            this.hideGroupBtn.Click += new System.EventHandler(this.hideGroupBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 26);
            this.label1.TabIndex = 10;
            this.label1.Text = "Hidden groups don\'t show in widget\r\nbut still trigger notifications.";
            // 
            // EditTimerGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 291);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(631, 330);
            this.Name = "EditTimerGroups";
            this.Text = "Modify timer groups";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox currentGroupsLb;
        private System.Windows.Forms.ComboBox serverGroupCb;
        private System.Windows.Forms.ComboBox playerCb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button removeBtn;
        private WinForms.LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button hideGroupBtn;
        private System.Windows.Forms.Label label1;

    }
}