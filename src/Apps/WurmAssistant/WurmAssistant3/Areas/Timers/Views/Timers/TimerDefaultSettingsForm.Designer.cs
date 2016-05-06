namespace AldursLab.WurmAssistant3.Areas.Timers.Views.Timers
{
    partial class TimerDefaultSettingsForm
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
            this.buttonTurnOff = new System.Windows.Forms.Button();
            this.checkBoxPopup = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelPopup = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownPopupDuration = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxOnAssistantLaunch = new System.Windows.Forms.CheckBox();
            this.checkBoxPopupPersistent = new System.Windows.Forms.CheckBox();
            this.panelSoundNotify = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSoundChange = new System.Windows.Forms.Button();
            this.textBoxSoundName = new System.Windows.Forms.TextBox();
            this.checkBoxSound = new System.Windows.Forms.CheckBox();
            this.buttonMoreOptions = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.panelPopup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopupDuration)).BeginInit();
            this.panelSoundNotify.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonTurnOff
            // 
            this.buttonTurnOff.Image = global::AldursLab.WurmAssistant3.Properties.Resources.DeleteRed_small;
            this.buttonTurnOff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTurnOff.Location = new System.Drawing.Point(199, 213);
            this.buttonTurnOff.Name = "buttonTurnOff";
            this.buttonTurnOff.Size = new System.Drawing.Size(154, 33);
            this.buttonTurnOff.TabIndex = 0;
            this.buttonTurnOff.TabStop = false;
            this.buttonTurnOff.Text = "Turn off";
            this.buttonTurnOff.UseVisualStyleBackColor = true;
            this.buttonTurnOff.Click += new System.EventHandler(this.buttonTurnOff_Click);
            // 
            // checkBoxPopup
            // 
            this.checkBoxPopup.AutoSize = true;
            this.checkBoxPopup.Location = new System.Drawing.Point(9, 21);
            this.checkBoxPopup.Name = "checkBoxPopup";
            this.checkBoxPopup.Size = new System.Drawing.Size(83, 21);
            this.checkBoxPopup.TabIndex = 1;
            this.checkBoxPopup.Text = "Popup...";
            this.checkBoxPopup.UseVisualStyleBackColor = true;
            this.checkBoxPopup.CheckedChanged += new System.EventHandler(this.checkBoxPopup_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelPopup);
            this.groupBox1.Controls.Add(this.panelSoundNotify);
            this.groupBox1.Controls.Add(this.checkBoxSound);
            this.groupBox1.Controls.Add(this.checkBoxPopup);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 195);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Notify me with...";
            // 
            // panelPopup
            // 
            this.panelPopup.Controls.Add(this.label2);
            this.panelPopup.Controls.Add(this.numericUpDownPopupDuration);
            this.panelPopup.Controls.Add(this.label1);
            this.panelPopup.Controls.Add(this.checkBoxOnAssistantLaunch);
            this.panelPopup.Controls.Add(this.checkBoxPopupPersistent);
            this.panelPopup.Location = new System.Drawing.Point(6, 48);
            this.panelPopup.Name = "panelPopup";
            this.panelPopup.Size = new System.Drawing.Size(172, 141);
            this.panelPopup.TabIndex = 5;
            this.panelPopup.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "seconds";
            // 
            // numericUpDownPopupDuration
            // 
            this.numericUpDownPopupDuration.Location = new System.Drawing.Point(3, 23);
            this.numericUpDownPopupDuration.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownPopupDuration.Name = "numericUpDownPopupDuration";
            this.numericUpDownPopupDuration.Size = new System.Drawing.Size(51, 22);
            this.numericUpDownPopupDuration.TabIndex = 3;
            this.numericUpDownPopupDuration.ValueChanged += new System.EventHandler(this.numericUpDownPopupDuration_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "which should stay";
            // 
            // checkBoxOnAssistantLaunch
            // 
            this.checkBoxOnAssistantLaunch.AutoSize = true;
            this.checkBoxOnAssistantLaunch.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxOnAssistantLaunch.Location = new System.Drawing.Point(3, 100);
            this.checkBoxOnAssistantLaunch.Name = "checkBoxOnAssistantLaunch";
            this.checkBoxOnAssistantLaunch.Size = new System.Drawing.Size(159, 38);
            this.checkBoxOnAssistantLaunch.TabIndex = 1;
            this.checkBoxOnAssistantLaunch.Text = "which should pop on\r\nassistant launch";
            this.checkBoxOnAssistantLaunch.UseVisualStyleBackColor = true;
            this.checkBoxOnAssistantLaunch.CheckedChanged += new System.EventHandler(this.checkBoxOnAssistantLaunch_CheckedChanged);
            // 
            // checkBoxPopupPersistent
            // 
            this.checkBoxPopupPersistent.AutoSize = true;
            this.checkBoxPopupPersistent.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxPopupPersistent.Location = new System.Drawing.Point(3, 56);
            this.checkBoxPopupPersistent.Name = "checkBoxPopupPersistent";
            this.checkBoxPopupPersistent.Size = new System.Drawing.Size(141, 38);
            this.checkBoxPopupPersistent.TabIndex = 0;
            this.checkBoxPopupPersistent.Text = "which should stay\r\nuntil clicked";
            this.checkBoxPopupPersistent.UseVisualStyleBackColor = true;
            this.checkBoxPopupPersistent.CheckedChanged += new System.EventHandler(this.checkBoxPopupPersistent_CheckedChanged);
            // 
            // panelSoundNotify
            // 
            this.panelSoundNotify.Controls.Add(this.tableLayoutPanel1);
            this.panelSoundNotify.Location = new System.Drawing.Point(184, 48);
            this.panelSoundNotify.Name = "panelSoundNotify";
            this.panelSoundNotify.Size = new System.Drawing.Size(154, 63);
            this.panelSoundNotify.TabIndex = 3;
            this.panelSoundNotify.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonSoundChange, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxSoundName, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(154, 63);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // buttonSoundChange
            // 
            this.buttonSoundChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSoundChange.Location = new System.Drawing.Point(3, 33);
            this.buttonSoundChange.Name = "buttonSoundChange";
            this.buttonSoundChange.Size = new System.Drawing.Size(148, 27);
            this.buttonSoundChange.TabIndex = 4;
            this.buttonSoundChange.Text = "Change";
            this.buttonSoundChange.UseVisualStyleBackColor = true;
            this.buttonSoundChange.Click += new System.EventHandler(this.buttonSoundChange_Click);
            // 
            // textBoxSoundName
            // 
            this.textBoxSoundName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSoundName.Location = new System.Drawing.Point(3, 3);
            this.textBoxSoundName.Name = "textBoxSoundName";
            this.textBoxSoundName.ReadOnly = true;
            this.textBoxSoundName.Size = new System.Drawing.Size(148, 22);
            this.textBoxSoundName.TabIndex = 3;
            // 
            // checkBoxSound
            // 
            this.checkBoxSound.AutoSize = true;
            this.checkBoxSound.Location = new System.Drawing.Point(187, 21);
            this.checkBoxSound.Name = "checkBoxSound";
            this.checkBoxSound.Size = new System.Drawing.Size(83, 21);
            this.checkBoxSound.TabIndex = 2;
            this.checkBoxSound.Text = "Sound...";
            this.checkBoxSound.UseVisualStyleBackColor = true;
            this.checkBoxSound.CheckedChanged += new System.EventHandler(this.checkBoxSound_CheckedChanged);
            // 
            // buttonMoreOptions
            // 
            this.buttonMoreOptions.Location = new System.Drawing.Point(21, 213);
            this.buttonMoreOptions.Name = "buttonMoreOptions";
            this.buttonMoreOptions.Size = new System.Drawing.Size(154, 33);
            this.buttonMoreOptions.TabIndex = 4;
            this.buttonMoreOptions.Text = "More options...";
            this.buttonMoreOptions.UseVisualStyleBackColor = true;
            this.buttonMoreOptions.Visible = false;
            this.buttonMoreOptions.Click += new System.EventHandler(this.buttonMoreOptions_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 150;
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 150;
            this.toolTip1.ReshowDelay = 30;
            // 
            // FormTimerSettingsDefault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 258);
            this.Controls.Add(this.buttonMoreOptions);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonTurnOff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimerDefaultSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTimerSettingsDefault";
            this.Load += new System.EventHandler(this.FormTimerSettingsDefault_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelPopup.ResumeLayout(false);
            this.panelPopup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopupDuration)).EndInit();
            this.panelSoundNotify.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonTurnOff;
        private System.Windows.Forms.CheckBox checkBoxPopup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelPopup;
        private System.Windows.Forms.CheckBox checkBoxPopupPersistent;
        private System.Windows.Forms.Panel panelSoundNotify;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonSoundChange;
        private System.Windows.Forms.TextBox textBoxSoundName;
        private System.Windows.Forms.CheckBox checkBoxSound;
        private System.Windows.Forms.Button buttonMoreOptions;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxOnAssistantLaunch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownPopupDuration;
        private System.Windows.Forms.Label label1;
    }
}