namespace AldursLab.WurmAssistant3.Areas.SoundManager.Views
{
    partial class SoundManagerView
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
            this.listBoxAllSounds = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonRename = new System.Windows.Forms.Button();
            this.textBoxSelectedSound = new System.Windows.Forms.TextBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarAdjustedVolume = new System.Windows.Forms.TrackBar();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonAddSounds = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.globalVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.irrklangImage = new System.Windows.Forms.PictureBox();
            this.sampleSoundPackLink = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAdjustedVolume)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalVolumeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.irrklangImage)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxAllSounds
            // 
            this.listBoxAllSounds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAllSounds.FormattingEnabled = true;
            this.listBoxAllSounds.Location = new System.Drawing.Point(5, 18);
            this.listBoxAllSounds.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxAllSounds.Name = "listBoxAllSounds";
            this.listBoxAllSounds.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAllSounds.Size = new System.Drawing.Size(308, 316);
            this.listBoxAllSounds.TabIndex = 0;
            this.listBoxAllSounds.SelectedIndexChanged += new System.EventHandler(this.listBoxAllSounds_SelectedIndexChanged);
            this.listBoxAllSounds.DoubleClick += new System.EventHandler(this.listBoxAllSounds_DoubleClick);
            this.listBoxAllSounds.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxAllSounds_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.buttonRename);
            this.groupBox1.Controls.Add(this.textBoxSelectedSound);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackBarAdjustedVolume);
            this.groupBox1.Controls.Add(this.buttonPlay);
            this.groupBox1.Location = new System.Drawing.Point(317, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(172, 316);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Sound";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 187);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 34);
            this.button1.TabIndex = 17;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonRename
            // 
            this.buttonRename.Enabled = false;
            this.buttonRename.Location = new System.Drawing.Point(8, 40);
            this.buttonRename.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(152, 34);
            this.buttonRename.TabIndex = 16;
            this.buttonRename.Text = "Rename";
            this.buttonRename.UseVisualStyleBackColor = true;
            this.buttonRename.Visible = false;
            this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
            // 
            // textBoxSelectedSound
            // 
            this.textBoxSelectedSound.Location = new System.Drawing.Point(8, 17);
            this.textBoxSelectedSound.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSelectedSound.Name = "textBoxSelectedSound";
            this.textBoxSelectedSound.ReadOnly = true;
            this.textBoxSelectedSound.Size = new System.Drawing.Size(152, 20);
            this.textBoxSelectedSound.TabIndex = 15;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(8, 276);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(152, 34);
            this.buttonRemove.TabIndex = 14;
            this.buttonRemove.Text = "Delete";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Volume fine-tuning";
            // 
            // trackBarAdjustedVolume
            // 
            this.trackBarAdjustedVolume.AutoSize = false;
            this.trackBarAdjustedVolume.Location = new System.Drawing.Point(8, 112);
            this.trackBarAdjustedVolume.Margin = new System.Windows.Forms.Padding(2);
            this.trackBarAdjustedVolume.Maximum = 100;
            this.trackBarAdjustedVolume.Name = "trackBarAdjustedVolume";
            this.trackBarAdjustedVolume.Size = new System.Drawing.Size(152, 31);
            this.trackBarAdjustedVolume.TabIndex = 12;
            this.trackBarAdjustedVolume.Value = 100;
            this.trackBarAdjustedVolume.ValueChanged += new System.EventHandler(this.trackBarAdjustedVolume_ValueChanged);
            this.trackBarAdjustedVolume.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trackBarAdjustedVolume_KeyDown);
            this.trackBarAdjustedVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarAdjustedVolume_MouseUp);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(8, 148);
            this.buttonPlay.Margin = new System.Windows.Forms.Padding(2);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(152, 34);
            this.buttonPlay.TabIndex = 0;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonAddSounds
            // 
            this.buttonAddSounds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddSounds.Location = new System.Drawing.Point(5, 338);
            this.buttonAddSounds.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddSounds.Name = "buttonAddSounds";
            this.buttonAddSounds.Size = new System.Drawing.Size(484, 34);
            this.buttonAddSounds.TabIndex = 3;
            this.buttonAddSounds.Text = "Add more sounds...";
            this.buttonAddSounds.UseVisualStyleBackColor = true;
            this.buttonAddSounds.Click += new System.EventHandler(this.buttonAddSounds_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Supported Sound Files|*.wav;*.ogg;*.mp3;*.flac;*.mod;*.it;*.s3d;*.xm";
            this.openFileDialog1.InitialDirectory = "SoundBank";
            this.openFileDialog1.Multiselect = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 15000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.sampleSoundPackLink);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.buttonAddSounds);
            this.groupBox2.Controls.Add(this.listBoxAllSounds);
            this.groupBox2.Location = new System.Drawing.Point(12, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(494, 402);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sounds";
            // 
            // globalVolumeTrackBar
            // 
            this.globalVolumeTrackBar.AutoSize = false;
            this.globalVolumeTrackBar.Location = new System.Drawing.Point(17, 33);
            this.globalVolumeTrackBar.Margin = new System.Windows.Forms.Padding(2);
            this.globalVolumeTrackBar.Maximum = 100;
            this.globalVolumeTrackBar.Name = "globalVolumeTrackBar";
            this.globalVolumeTrackBar.Size = new System.Drawing.Size(308, 31);
            this.globalVolumeTrackBar.TabIndex = 18;
            this.globalVolumeTrackBar.Value = 100;
            this.globalVolumeTrackBar.Scroll += new System.EventHandler(this.globalVolumeTrackBar_Scroll);
            this.globalVolumeTrackBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.globalVolumeTrackBar_KeyDown);
            this.globalVolumeTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.globalVolumeTrackBar_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Global volume:";
            // 
            // irrklangImage
            // 
            this.irrklangImage.Image = global::AldursLab.WurmAssistant3.Properties.Resources.irrklang_small;
            this.irrklangImage.Location = new System.Drawing.Point(337, 18);
            this.irrklangImage.Name = "irrklangImage";
            this.irrklangImage.Size = new System.Drawing.Size(169, 50);
            this.irrklangImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.irrklangImage.TabIndex = 20;
            this.irrklangImage.TabStop = false;
            // 
            // sampleSoundPackLink
            // 
            this.sampleSoundPackLink.AutoSize = true;
            this.sampleSoundPackLink.Location = new System.Drawing.Point(5, 378);
            this.sampleSoundPackLink.Name = "sampleSoundPackLink";
            this.sampleSoundPackLink.Size = new System.Drawing.Size(159, 13);
            this.sampleSoundPackLink.TabIndex = 4;
            this.sampleSoundPackLink.TabStop = true;
            this.sampleSoundPackLink.Text = "Download sample sound pack...";
            this.sampleSoundPackLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.sampleSoundPackLink_LinkClicked);
            // 
            // SoundManagerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 483);
            this.Controls.Add(this.irrklangImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.globalVolumeTrackBar);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(534, 437);
            this.Name = "SoundManagerView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sounds Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SoundManagerView_FormClosing);
            this.Load += new System.EventHandler(this.FormSoundBank_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAdjustedVolume)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalVolumeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.irrklangImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAllSounds;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarAdjustedVolume;
        private System.Windows.Forms.Button buttonAddSounds;
        private System.Windows.Forms.TextBox textBoxSelectedSound;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar globalVolumeTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox irrklangImage;
        private System.Windows.Forms.LinkLabel sampleSoundPackLink;

    }
}