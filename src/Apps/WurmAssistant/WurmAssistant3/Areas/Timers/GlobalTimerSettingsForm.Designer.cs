using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    partial class GlobalTimerSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonResetWidgetDefaultColor = new System.Windows.Forms.Button();
            this.labelAutowrap2 = new AldursLab.WurmAssistant3.Utils.WinForms.LabelAutowrap();
            this.textBoxWidgetSample = new System.Windows.Forms.TextBox();
            this.buttonSetWidgetFontColor = new System.Windows.Forms.Button();
            this.buttonChangeWidgetBgColor = new System.Windows.Forms.Button();
            this.checkBoxWidgetView = new System.Windows.Forms.CheckBox();
            this.labelAutowrap1 = new AldursLab.WurmAssistant3.Utils.WinForms.LabelAutowrap();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.checkBoxShowEndDate = new System.Windows.Forms.CheckBox();
            this.checkBoxShowEndDateInstead = new System.Windows.Forms.CheckBox();
            this.grpBarColorMode = new System.Windows.Forms.GroupBox();
            this.rbnColorRedReady = new System.Windows.Forms.RadioButton();
            this.rbnColorGreenReady = new System.Windows.Forms.RadioButton();
            this.rbnColorSimple = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.grpBarColorMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonResetWidgetDefaultColor);
            this.groupBox1.Controls.Add(this.labelAutowrap2);
            this.groupBox1.Controls.Add(this.textBoxWidgetSample);
            this.groupBox1.Controls.Add(this.buttonSetWidgetFontColor);
            this.groupBox1.Controls.Add(this.buttonChangeWidgetBgColor);
            this.groupBox1.Controls.Add(this.checkBoxWidgetView);
            this.groupBox1.Controls.Add(this.labelAutowrap1);
            this.groupBox1.Location = new System.Drawing.Point(2, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 262);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Alternative view";
            // 
            // buttonResetWidgetDefaultColor
            // 
            this.buttonResetWidgetDefaultColor.Location = new System.Drawing.Point(6, 229);
            this.buttonResetWidgetDefaultColor.Name = "buttonResetWidgetDefaultColor";
            this.buttonResetWidgetDefaultColor.Size = new System.Drawing.Size(211, 23);
            this.buttonResetWidgetDefaultColor.TabIndex = 11;
            this.buttonResetWidgetDefaultColor.Text = "Reset default";
            this.buttonResetWidgetDefaultColor.UseVisualStyleBackColor = true;
            this.buttonResetWidgetDefaultColor.Click += new System.EventHandler(this.buttonResetWidgetDefaultColor_Click);
            // 
            // labelAutowrap2
            // 
            this.labelAutowrap2.Location = new System.Drawing.Point(6, 114);
            this.labelAutowrap2.Name = "labelAutowrap2";
            this.labelAutowrap2.Size = new System.Drawing.Size(248, 13);
            this.labelAutowrap2.TabIndex = 10;
            this.labelAutowrap2.Text = "Timers colors while in widget mode:";
            // 
            // textBoxWidgetSample
            // 
            this.textBoxWidgetSample.Location = new System.Drawing.Point(6, 130);
            this.textBoxWidgetSample.Multiline = true;
            this.textBoxWidgetSample.Name = "textBoxWidgetSample";
            this.textBoxWidgetSample.ReadOnly = true;
            this.textBoxWidgetSample.Size = new System.Drawing.Size(211, 36);
            this.textBoxWidgetSample.TabIndex = 9;
            this.textBoxWidgetSample.Text = "some sample text";
            this.textBoxWidgetSample.TextChanged += new System.EventHandler(this.textBoxWidgetSample_TextChanged);
            // 
            // buttonSetWidgetFontColor
            // 
            this.buttonSetWidgetFontColor.Location = new System.Drawing.Point(6, 201);
            this.buttonSetWidgetFontColor.Name = "buttonSetWidgetFontColor";
            this.buttonSetWidgetFontColor.Size = new System.Drawing.Size(211, 23);
            this.buttonSetWidgetFontColor.TabIndex = 8;
            this.buttonSetWidgetFontColor.Text = "Change font color";
            this.buttonSetWidgetFontColor.UseVisualStyleBackColor = true;
            this.buttonSetWidgetFontColor.Click += new System.EventHandler(this.buttonSetWidgetFontColor_Click);
            // 
            // buttonChangeWidgetBgColor
            // 
            this.buttonChangeWidgetBgColor.Location = new System.Drawing.Point(6, 172);
            this.buttonChangeWidgetBgColor.Name = "buttonChangeWidgetBgColor";
            this.buttonChangeWidgetBgColor.Size = new System.Drawing.Size(211, 23);
            this.buttonChangeWidgetBgColor.TabIndex = 5;
            this.buttonChangeWidgetBgColor.Text = "Change background color";
            this.buttonChangeWidgetBgColor.UseVisualStyleBackColor = true;
            this.buttonChangeWidgetBgColor.Click += new System.EventHandler(this.buttonChangeWidgetBgColor_Click);
            // 
            // checkBoxWidgetView
            // 
            this.checkBoxWidgetView.AutoSize = true;
            this.checkBoxWidgetView.Location = new System.Drawing.Point(6, 19);
            this.checkBoxWidgetView.Name = "checkBoxWidgetView";
            this.checkBoxWidgetView.Size = new System.Drawing.Size(118, 17);
            this.checkBoxWidgetView.TabIndex = 1;
            this.checkBoxWidgetView.Text = "Enable widget view";
            this.checkBoxWidgetView.UseVisualStyleBackColor = true;
            this.checkBoxWidgetView.CheckedChanged += new System.EventHandler(this.checkBoxWidgetView_CheckedChanged);
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Location = new System.Drawing.Point(6, 39);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(248, 52);
            this.labelAutowrap1.TabIndex = 0;
            this.labelAutowrap1.Text = "If enabled, middle mouse button will swap between normal window and a widget more" +
    " suitable to place on top of Wurm game window.\r\nWhile in widget mode, timers alw" +
    "ays stay on top.";
            // 
            // colorDialog1
            // 
            this.colorDialog1.SolidColorOnly = true;
            // 
            // checkBoxShowEndDate
            // 
            this.checkBoxShowEndDate.AutoSize = true;
            this.checkBoxShowEndDate.Location = new System.Drawing.Point(286, 98);
            this.checkBoxShowEndDate.Name = "checkBoxShowEndDate";
            this.checkBoxShowEndDate.Size = new System.Drawing.Size(238, 30);
            this.checkBoxShowEndDate.TabIndex = 1;
            this.checkBoxShowEndDate.Text = "Show time of day of when the timer will finish,\r\nin local time, next to time rema" +
    "ining\r\n";
            this.checkBoxShowEndDate.UseVisualStyleBackColor = true;
            this.checkBoxShowEndDate.CheckedChanged += new System.EventHandler(this.checkBoxShowEndDate_CheckedChanged);
            // 
            // checkBoxShowEndDateInstead
            // 
            this.checkBoxShowEndDateInstead.AutoSize = true;
            this.checkBoxShowEndDateInstead.Location = new System.Drawing.Point(303, 131);
            this.checkBoxShowEndDateInstead.Name = "checkBoxShowEndDateInstead";
            this.checkBoxShowEndDateInstead.Size = new System.Drawing.Size(172, 17);
            this.checkBoxShowEndDateInstead.TabIndex = 2;
            this.checkBoxShowEndDateInstead.Text = "Show instead of remaining time\r\n";
            this.checkBoxShowEndDateInstead.UseVisualStyleBackColor = true;
            this.checkBoxShowEndDateInstead.CheckedChanged += new System.EventHandler(this.checkBoxShowEndDateInstead_CheckedChanged);
            // 
            // grpBarColorMode
            // 
            this.grpBarColorMode.Controls.Add(this.rbnColorRedReady);
            this.grpBarColorMode.Controls.Add(this.rbnColorGreenReady);
            this.grpBarColorMode.Controls.Add(this.rbnColorSimple);
            this.grpBarColorMode.Location = new System.Drawing.Point(273, 1);
            this.grpBarColorMode.Name = "grpBarColorMode";
            this.grpBarColorMode.Size = new System.Drawing.Size(265, 91);
            this.grpBarColorMode.TabIndex = 17;
            this.grpBarColorMode.TabStop = false;
            this.grpBarColorMode.Text = "Bar colors";
            // 
            // rbnColorRedReady
            // 
            this.rbnColorRedReady.AutoSize = true;
            this.rbnColorRedReady.Location = new System.Drawing.Point(13, 65);
            this.rbnColorRedReady.Name = "rbnColorRedReady";
            this.rbnColorRedReady.Size = new System.Drawing.Size(166, 17);
            this.rbnColorRedReady.TabIndex = 2;
            this.rbnColorRedReady.TabStop = true;
            this.rbnColorRedReady.Tag = "2";
            this.rbnColorRedReady.Text = "colorful, bar is red when ready";
            this.rbnColorRedReady.UseVisualStyleBackColor = true;
            this.rbnColorRedReady.CheckedChanged += new System.EventHandler(this.rbnColor_CheckedChanged);
            // 
            // rbnColorGreenReady
            // 
            this.rbnColorGreenReady.AutoSize = true;
            this.rbnColorGreenReady.Location = new System.Drawing.Point(13, 42);
            this.rbnColorGreenReady.Name = "rbnColorGreenReady";
            this.rbnColorGreenReady.Size = new System.Drawing.Size(178, 17);
            this.rbnColorGreenReady.TabIndex = 1;
            this.rbnColorGreenReady.TabStop = true;
            this.rbnColorGreenReady.Tag = "1";
            this.rbnColorGreenReady.Text = "colorful, bar is green when ready";
            this.rbnColorGreenReady.UseVisualStyleBackColor = true;
            this.rbnColorGreenReady.CheckedChanged += new System.EventHandler(this.rbnColor_CheckedChanged);
            // 
            // rbnColorSimple
            // 
            this.rbnColorSimple.AutoSize = true;
            this.rbnColorSimple.Location = new System.Drawing.Point(13, 19);
            this.rbnColorSimple.Name = "rbnColorSimple";
            this.rbnColorSimple.Size = new System.Drawing.Size(135, 17);
            this.rbnColorSimple.TabIndex = 0;
            this.rbnColorSimple.TabStop = true;
            this.rbnColorSimple.Tag = "0";
            this.rbnColorSimple.Text = "simple (only green bars)";
            this.rbnColorSimple.UseVisualStyleBackColor = true;
            this.rbnColorSimple.CheckedChanged += new System.EventHandler(this.rbnColor_CheckedChanged);
            // 
            // GlobalTimerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 267);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpBarColorMode);
            this.Controls.Add(this.checkBoxShowEndDateInstead);
            this.Controls.Add(this.checkBoxShowEndDate);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalTimerSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timers settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpBarColorMode.ResumeLayout(false);
            this.grpBarColorMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxWidgetView;
        private LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button buttonChangeWidgetBgColor;
        private System.Windows.Forms.Button buttonSetWidgetFontColor;
        private System.Windows.Forms.TextBox textBoxWidgetSample;
        private LabelAutowrap labelAutowrap2;
        private System.Windows.Forms.Button buttonResetWidgetDefaultColor;
        private System.Windows.Forms.CheckBox checkBoxShowEndDate;
        private System.Windows.Forms.CheckBox checkBoxShowEndDateInstead;
        private System.Windows.Forms.GroupBox grpBarColorMode;
        private System.Windows.Forms.RadioButton rbnColorRedReady;
        private System.Windows.Forms.RadioButton rbnColorGreenReady;
        private System.Windows.Forms.RadioButton rbnColorSimple;
    }
}