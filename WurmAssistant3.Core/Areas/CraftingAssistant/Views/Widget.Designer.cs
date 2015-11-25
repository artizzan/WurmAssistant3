namespace AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant.Views
{
    partial class Widget
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
            this.widgetHelpLbl = new System.Windows.Forms.Label();
            this.actionLbl = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // widgetHelpLbl
            // 
            this.widgetHelpLbl.AutoSize = true;
            this.widgetHelpLbl.Location = new System.Drawing.Point(12, 55);
            this.widgetHelpLbl.Name = "widgetHelpLbl";
            this.widgetHelpLbl.Size = new System.Drawing.Size(258, 13);
            this.widgetHelpLbl.TabIndex = 0;
            this.widgetHelpLbl.Text = "middle mouse click to flip into and out of widget mode";
            // 
            // actionLbl
            // 
            this.actionLbl.AutoSize = true;
            this.actionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.actionLbl.Location = new System.Drawing.Point(3, 0);
            this.actionLbl.Name = "actionLbl";
            this.actionLbl.Size = new System.Drawing.Size(202, 25);
            this.actionLbl.TabIndex = 1;
            this.actionLbl.Text = "Tool name or action";
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.actionLbl);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 31);
            this.panel1.TabIndex = 2;
            // 
            // Widget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 83);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.widgetHelpLbl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Widget";
            this.ShowIcon = false;
            this.Text = "Widget";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Widget_FormClosing);
            this.Load += new System.EventHandler(this.Widget_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label widgetHelpLbl;
        private System.Windows.Forms.Label actionLbl;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Panel panel1;
    }
}