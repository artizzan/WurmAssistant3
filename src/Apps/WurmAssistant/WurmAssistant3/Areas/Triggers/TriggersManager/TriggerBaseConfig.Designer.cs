using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    partial class TriggerBaseConfig
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TriggerNameTbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LogTypesChklist = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CooldownInput = new AldursLab.WurmAssistant3.Utils.WinForms.TimeSpanInputCompact();
            this.ApplicableLogsDisplayTxtbox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.CooldownEnabledChkbox = new System.Windows.Forms.CheckBox();
            this.ResetOnCndHitChkbox = new System.Windows.Forms.CheckBox();
            this.DelayInput = new AldursLab.WurmAssistant3.Utils.WinForms.TimeSpanInputCompact();
            this.label4 = new System.Windows.Forms.Label();
            this.DelayChkbox = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkStateRenderer1 = new BrightIdeasSoftware.CheckStateRenderer();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TriggerNameTbox
            // 
            this.TriggerNameTbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TriggerNameTbox.Location = new System.Drawing.Point(93, 3);
            this.TriggerNameTbox.Name = "TriggerNameTbox";
            this.TriggerNameTbox.Size = new System.Drawing.Size(224, 20);
            this.TriggerNameTbox.TabIndex = 0;
            this.TriggerNameTbox.TextChanged += new System.EventHandler(this.TriggerNameTbox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Trigger name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TriggerNameTbox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LogTypesChklist, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.CooldownInput, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.ApplicableLogsDisplayTxtbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.DelayInput, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.DelayChkbox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 1, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(320, 300);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // LogTypesChklist
            // 
            this.LogTypesChklist.CheckOnClick = true;
            this.LogTypesChklist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTypesChklist.FormattingEnabled = true;
            this.LogTypesChklist.Location = new System.Drawing.Point(93, 51);
            this.LogTypesChklist.Name = "LogTypesChklist";
            this.LogTypesChklist.Size = new System.Drawing.Size(224, 126);
            this.LogTypesChklist.Sorted = true;
            this.LogTypesChklist.TabIndex = 2;
            this.LogTypesChklist.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LogTypesChklist_ItemCheck);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Apply to logs:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(0, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cooldown:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CooldownInput
            // 
            this.CooldownInput.Location = new System.Drawing.Point(92, 206);
            this.CooldownInput.Margin = new System.Windows.Forms.Padding(2);
            this.CooldownInput.Name = "CooldownInput";
            this.CooldownInput.ReadOnly = false;
            this.CooldownInput.Size = new System.Drawing.Size(170, 20);
            this.CooldownInput.TabIndex = 7;
            this.CooldownInput.Value = System.TimeSpan.Parse("00:00:00");
            this.CooldownInput.ValueChanged += new System.EventHandler(this.CooldownInput_ValueChanged);
            // 
            // ApplicableLogsDisplayTxtbox
            // 
            this.ApplicableLogsDisplayTxtbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicableLogsDisplayTxtbox.Location = new System.Drawing.Point(93, 27);
            this.ApplicableLogsDisplayTxtbox.Name = "ApplicableLogsDisplayTxtbox";
            this.ApplicableLogsDisplayTxtbox.ReadOnly = true;
            this.ApplicableLogsDisplayTxtbox.Size = new System.Drawing.Size(224, 20);
            this.ApplicableLogsDisplayTxtbox.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.CooldownEnabledChkbox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ResetOnCndHitChkbox, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(90, 180);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(230, 24);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // CooldownEnabledChkbox
            // 
            this.CooldownEnabledChkbox.AutoSize = true;
            this.CooldownEnabledChkbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CooldownEnabledChkbox.Location = new System.Drawing.Point(3, 3);
            this.CooldownEnabledChkbox.Name = "CooldownEnabledChkbox";
            this.CooldownEnabledChkbox.Size = new System.Drawing.Size(74, 18);
            this.CooldownEnabledChkbox.TabIndex = 6;
            this.CooldownEnabledChkbox.Text = "enable";
            this.CooldownEnabledChkbox.UseVisualStyleBackColor = true;
            this.CooldownEnabledChkbox.CheckedChanged += new System.EventHandler(this.CooldownEnabledChkbox_CheckedChanged);
            // 
            // ResetOnCndHitChkbox
            // 
            this.ResetOnCndHitChkbox.AutoSize = true;
            this.ResetOnCndHitChkbox.Location = new System.Drawing.Point(83, 3);
            this.ResetOnCndHitChkbox.Name = "ResetOnCndHitChkbox";
            this.ResetOnCndHitChkbox.Size = new System.Drawing.Size(84, 17);
            this.ResetOnCndHitChkbox.TabIndex = 7;
            this.ResetOnCndHitChkbox.Text = "always reset";
            this.ResetOnCndHitChkbox.UseVisualStyleBackColor = true;
            this.ResetOnCndHitChkbox.CheckedChanged += new System.EventHandler(this.ResetOnCndHitChkbox_CheckedChanged);
            // 
            // DelayInput
            // 
            this.DelayInput.Location = new System.Drawing.Point(92, 254);
            this.DelayInput.Margin = new System.Windows.Forms.Padding(2);
            this.DelayInput.Name = "DelayInput";
            this.DelayInput.ReadOnly = false;
            this.DelayInput.Size = new System.Drawing.Size(170, 20);
            this.DelayInput.TabIndex = 10;
            this.DelayInput.Value = System.TimeSpan.Parse("00:00:00");
            this.DelayInput.ValueChanged += new System.EventHandler(this.DelayInput_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(0, 228);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 24);
            this.label4.TabIndex = 11;
            this.label4.Text = "Delay:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DelayChkbox
            // 
            this.DelayChkbox.AutoSize = true;
            this.DelayChkbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DelayChkbox.Location = new System.Drawing.Point(93, 232);
            this.DelayChkbox.Name = "DelayChkbox";
            this.DelayChkbox.Size = new System.Drawing.Size(224, 17);
            this.DelayChkbox.TabIndex = 12;
            this.DelayChkbox.Text = "enable";
            this.DelayChkbox.UseVisualStyleBackColor = true;
            this.DelayChkbox.CheckedChanged += new System.EventHandler(this.DelayChkbox_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel1.Location = new System.Drawing.Point(93, 276);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(224, 24);
            this.linkLabel1.TabIndex = 13;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "What\'s this cooldown and delay for?";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 20000;
            this.toolTip1.InitialDelay = 200;
            this.toolTip1.ReshowDelay = 100;
            // 
            // TriggerBaseConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TriggerBaseConfig";
            this.Size = new System.Drawing.Size(320, 300);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TriggerNameTbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckedListBox LogTypesChklist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private TimeSpanInputCompact CooldownInput;
        private System.Windows.Forms.TextBox ApplicableLogsDisplayTxtbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox CooldownEnabledChkbox;
        private System.Windows.Forms.CheckBox ResetOnCndHitChkbox;
        private System.Windows.Forms.ToolTip toolTip1;
        private TimeSpanInputCompact DelayInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox DelayChkbox;
        private BrightIdeasSoftware.CheckStateRenderer checkStateRenderer1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
