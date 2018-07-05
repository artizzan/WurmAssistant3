namespace AldursLab.WurmAssistant.Launcher.Views
{
    partial class ChooseApp
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
            this.stableWinBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.betaBtn = new System.Windows.Forms.Button();
            this.stableLinBtn = new System.Windows.Forms.Button();
            this.stableMacBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.unlimBetaBtn = new System.Windows.Forms.Button();
            this.unlimStableLinBtn = new System.Windows.Forms.Button();
            this.unlimStableMacBtn = new System.Windows.Forms.Button();
            this.unlimStableWinBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkAllBtn = new System.Windows.Forms.Button();
            this.specificBuildNumberNb = new System.Windows.Forms.NumericUpDown();
            this.useSpecificBuildNumberCb = new System.Windows.Forms.CheckBox();
            this.relativeDataDirPathCb = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specificBuildNumberNb)).BeginInit();
            this.SuspendLayout();
            // 
            // stableWinBtn
            // 
            this.stableWinBtn.Location = new System.Drawing.Point(18, 19);
            this.stableWinBtn.Name = "stableWinBtn";
            this.stableWinBtn.Size = new System.Drawing.Size(107, 43);
            this.stableWinBtn.TabIndex = 0;
            this.stableWinBtn.Text = "Stable \r\n(Windows)";
            this.stableWinBtn.UseVisualStyleBackColor = true;
            this.stableWinBtn.Click += new System.EventHandler(this.stableWinBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.betaBtn);
            this.groupBox1.Controls.Add(this.stableLinBtn);
            this.groupBox1.Controls.Add(this.stableMacBtn);
            this.groupBox1.Controls.Add(this.stableWinBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(442, 79);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wurm Assistant for Wurm Online";
            // 
            // betaBtn
            // 
            this.betaBtn.Enabled = false;
            this.betaBtn.Location = new System.Drawing.Point(361, 19);
            this.betaBtn.Name = "betaBtn";
            this.betaBtn.Size = new System.Drawing.Size(75, 43);
            this.betaBtn.TabIndex = 3;
            this.betaBtn.Text = "Beta";
            this.betaBtn.UseVisualStyleBackColor = true;
            this.betaBtn.Click += new System.EventHandler(this.betaBtn_Click);
            // 
            // stableLinBtn
            // 
            this.stableLinBtn.Enabled = false;
            this.stableLinBtn.Location = new System.Drawing.Point(131, 19);
            this.stableLinBtn.Name = "stableLinBtn";
            this.stableLinBtn.Size = new System.Drawing.Size(107, 43);
            this.stableLinBtn.TabIndex = 1;
            this.stableLinBtn.Text = "Stable \r\n(Linux)";
            this.stableLinBtn.UseVisualStyleBackColor = true;
            this.stableLinBtn.Click += new System.EventHandler(this.stableLinBtn_Click);
            // 
            // stableMacBtn
            // 
            this.stableMacBtn.Enabled = false;
            this.stableMacBtn.Location = new System.Drawing.Point(244, 19);
            this.stableMacBtn.Name = "stableMacBtn";
            this.stableMacBtn.Size = new System.Drawing.Size(111, 43);
            this.stableMacBtn.TabIndex = 2;
            this.stableMacBtn.Text = "Stable \r\n(Mac)";
            this.stableMacBtn.UseVisualStyleBackColor = true;
            this.stableMacBtn.Click += new System.EventHandler(this.stableMacBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.unlimBetaBtn);
            this.groupBox2.Controls.Add(this.unlimStableLinBtn);
            this.groupBox2.Controls.Add(this.unlimStableMacBtn);
            this.groupBox2.Controls.Add(this.unlimStableWinBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 79);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Wurm Assistant for Wurm Unlimited";
            // 
            // unlimBetaBtn
            // 
            this.unlimBetaBtn.Enabled = false;
            this.unlimBetaBtn.Location = new System.Drawing.Point(361, 19);
            this.unlimBetaBtn.Name = "unlimBetaBtn";
            this.unlimBetaBtn.Size = new System.Drawing.Size(75, 43);
            this.unlimBetaBtn.TabIndex = 8;
            this.unlimBetaBtn.Text = "Beta";
            this.unlimBetaBtn.UseVisualStyleBackColor = true;
            this.unlimBetaBtn.Click += new System.EventHandler(this.unlimBetaBtn_Click);
            // 
            // unlimStableLinBtn
            // 
            this.unlimStableLinBtn.Enabled = false;
            this.unlimStableLinBtn.Location = new System.Drawing.Point(131, 19);
            this.unlimStableLinBtn.Name = "unlimStableLinBtn";
            this.unlimStableLinBtn.Size = new System.Drawing.Size(107, 43);
            this.unlimStableLinBtn.TabIndex = 6;
            this.unlimStableLinBtn.Text = "Stable \r\n(Linux)";
            this.unlimStableLinBtn.UseVisualStyleBackColor = true;
            this.unlimStableLinBtn.Click += new System.EventHandler(this.unlimStableLinBtn_Click);
            // 
            // unlimStableMacBtn
            // 
            this.unlimStableMacBtn.Enabled = false;
            this.unlimStableMacBtn.Location = new System.Drawing.Point(244, 19);
            this.unlimStableMacBtn.Name = "unlimStableMacBtn";
            this.unlimStableMacBtn.Size = new System.Drawing.Size(111, 43);
            this.unlimStableMacBtn.TabIndex = 7;
            this.unlimStableMacBtn.Text = "Stable \r\n(Mac)";
            this.unlimStableMacBtn.UseVisualStyleBackColor = true;
            this.unlimStableMacBtn.Click += new System.EventHandler(this.unlimStableMacBtn_Click);
            // 
            // unlimStableWinBtn
            // 
            this.unlimStableWinBtn.Location = new System.Drawing.Point(18, 19);
            this.unlimStableWinBtn.Name = "unlimStableWinBtn";
            this.unlimStableWinBtn.Size = new System.Drawing.Size(107, 43);
            this.unlimStableWinBtn.TabIndex = 5;
            this.unlimStableWinBtn.Text = "Stable \r\n(Windows)";
            this.unlimStableWinBtn.UseVisualStyleBackColor = true;
            this.unlimStableWinBtn.Click += new System.EventHandler(this.unlimStableWinBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkAllBtn);
            this.groupBox3.Controls.Add(this.specificBuildNumberNb);
            this.groupBox3.Controls.Add(this.useSpecificBuildNumberCb);
            this.groupBox3.Controls.Add(this.relativeDataDirPathCb);
            this.groupBox3.Location = new System.Drawing.Point(12, 182);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(442, 81);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced options";
            // 
            // checkAllBtn
            // 
            this.checkAllBtn.Location = new System.Drawing.Point(235, 42);
            this.checkAllBtn.Name = "checkAllBtn";
            this.checkAllBtn.Size = new System.Drawing.Size(140, 20);
            this.checkAllBtn.TabIndex = 13;
            this.checkAllBtn.Text = "Check all packages";
            this.checkAllBtn.UseVisualStyleBackColor = true;
            this.checkAllBtn.Click += new System.EventHandler(this.checkAllBtn_Click);
            // 
            // specificBuildNumberNb
            // 
            this.specificBuildNumberNb.Location = new System.Drawing.Point(160, 42);
            this.specificBuildNumberNb.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.specificBuildNumberNb.Name = "specificBuildNumberNb";
            this.specificBuildNumberNb.Size = new System.Drawing.Size(69, 20);
            this.specificBuildNumberNb.TabIndex = 12;
            this.specificBuildNumberNb.ValueChanged += new System.EventHandler(this.specificBuildNumberNb_ValueChanged);
            // 
            // useSpecificBuildNumberCb
            // 
            this.useSpecificBuildNumberCb.AutoSize = true;
            this.useSpecificBuildNumberCb.Location = new System.Drawing.Point(7, 43);
            this.useSpecificBuildNumberCb.Name = "useSpecificBuildNumberCb";
            this.useSpecificBuildNumberCb.Size = new System.Drawing.Size(147, 17);
            this.useSpecificBuildNumberCb.TabIndex = 11;
            this.useSpecificBuildNumberCb.Text = "Use specific build number";
            this.useSpecificBuildNumberCb.UseVisualStyleBackColor = true;
            this.useSpecificBuildNumberCb.CheckedChanged += new System.EventHandler(this.useSpecificBuildNumberCb_CheckedChanged);
            // 
            // relativeDataDirPathCb
            // 
            this.relativeDataDirPathCb.AutoSize = true;
            this.relativeDataDirPathCb.Location = new System.Drawing.Point(7, 20);
            this.relativeDataDirPathCb.Name = "relativeDataDirPathCb";
            this.relativeDataDirPathCb.Size = new System.Drawing.Size(368, 17);
            this.relativeDataDirPathCb.TabIndex = 10;
            this.relativeDataDirPathCb.Text = "Use relative data directory (will stay in launcher directory - portable mode)";
            this.relativeDataDirPathCb.UseVisualStyleBackColor = true;
            this.relativeDataDirPathCb.CheckedChanged += new System.EventHandler(this.relativeDataDirPathCb_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(402, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "You can skip this screen. Use .bat files or check README.txt for advanced startup" +
    ".";
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ChooseApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 298);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseApp";
            this.ShowIcon = false;
            this.Text = "Choose Your Assistant!";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specificBuildNumberNb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button stableWinBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button betaBtn;
        private System.Windows.Forms.Button stableLinBtn;
        private System.Windows.Forms.Button stableMacBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button unlimBetaBtn;
        private System.Windows.Forms.Button unlimStableLinBtn;
        private System.Windows.Forms.Button unlimStableMacBtn;
        private System.Windows.Forms.Button unlimStableWinBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox useSpecificBuildNumberCb;
        private System.Windows.Forms.CheckBox relativeDataDirPathCb;
        private System.Windows.Forms.NumericUpDown specificBuildNumberNb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button checkAllBtn;
        private System.Windows.Forms.Timer timer1;
    }
}