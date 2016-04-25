namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Views
{
    partial class CombatWidgetView
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
            this.currentAttackersLbl = new System.Windows.Forms.Label();
            this.currentFocusLbl = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.widgetHelpLbl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // currentAttackersLbl
            // 
            this.currentAttackersLbl.AutoSize = true;
            this.currentAttackersLbl.Location = new System.Drawing.Point(97, 11);
            this.currentAttackersLbl.Name = "currentAttackersLbl";
            this.currentAttackersLbl.Size = new System.Drawing.Size(80, 13);
            this.currentAttackersLbl.TabIndex = 0;
            this.currentAttackersLbl.Text = "attacker names";
            // 
            // currentFocusLbl
            // 
            this.currentFocusLbl.AutoSize = true;
            this.currentFocusLbl.Location = new System.Drawing.Point(97, 38);
            this.currentFocusLbl.Name = "currentFocusLbl";
            this.currentFocusLbl.Size = new System.Drawing.Size(58, 13);
            this.currentFocusLbl.TabIndex = 1;
            this.currentFocusLbl.Text = "focus level";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 50;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Focus:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "New attackers:";
            // 
            // widgetHelpLbl
            // 
            this.widgetHelpLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.widgetHelpLbl.AutoSize = true;
            this.widgetHelpLbl.Location = new System.Drawing.Point(38, 78);
            this.widgetHelpLbl.Name = "widgetHelpLbl";
            this.widgetHelpLbl.Size = new System.Drawing.Size(258, 13);
            this.widgetHelpLbl.TabIndex = 4;
            this.widgetHelpLbl.Text = "middle mouse click to flip into and out of widget mode";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.currentAttackersLbl);
            this.panel1.Controls.Add(this.currentFocusLbl);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(319, 63);
            this.panel1.TabIndex = 5;
            // 
            // CombatAssistantView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 103);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.widgetHelpLbl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CombatWidgetView";
            this.ShowIcon = false;
            this.Text = "CombatAssistantView";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label currentAttackersLbl;
        private System.Windows.Forms.Label currentFocusLbl;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label widgetHelpLbl;
        private System.Windows.Forms.Panel panel1;
    }
}