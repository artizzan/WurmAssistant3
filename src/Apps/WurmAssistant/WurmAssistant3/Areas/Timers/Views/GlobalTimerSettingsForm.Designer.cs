using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Views
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
            this.labelAutowrap2 = new LabelAutowrap();
            this.textBoxWidgetSample = new System.Windows.Forms.TextBox();
            this.buttonSetWidgetFontColor = new System.Windows.Forms.Button();
            this.buttonChangeWidgetBgColor = new System.Windows.Forms.Button();
            this.checkBoxWidgetView = new System.Windows.Forms.CheckBox();
            this.labelAutowrap1 = new LabelAutowrap();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.checkBoxShowEndDate = new System.Windows.Forms.CheckBox();
            this.checkBoxShowEndDateInstead = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 88);
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
            this.checkBoxShowEndDate.Location = new System.Drawing.Point(12, 12);
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
            this.checkBoxShowEndDateInstead.Location = new System.Drawing.Point(31, 48);
            this.checkBoxShowEndDateInstead.Name = "checkBoxShowEndDateInstead";
            this.checkBoxShowEndDateInstead.Size = new System.Drawing.Size(172, 17);
            this.checkBoxShowEndDateInstead.TabIndex = 2;
            this.checkBoxShowEndDateInstead.Text = "Show instead of remaining time\r\n";
            this.checkBoxShowEndDateInstead.UseVisualStyleBackColor = true;
            this.checkBoxShowEndDateInstead.CheckedChanged += new System.EventHandler(this.checkBoxShowEndDateInstead_CheckedChanged);
            // 
            // GlobalTimerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 362);
            this.Controls.Add(this.checkBoxShowEndDateInstead);
            this.Controls.Add(this.checkBoxShowEndDate);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalTimerSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timers settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
    }
}