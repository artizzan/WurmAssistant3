namespace AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Views
{
    partial class FormPopupContainer
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
            this.popupNotifier1 = new PopupNotifier();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // popupNotifier1
            // 
            this.popupNotifier1.AnimationDuration = 250;
            this.popupNotifier1.ContentFont = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.popupNotifier1.ContentText = null;
            this.popupNotifier1.HeaderHeight = 12;
            this.popupNotifier1.Image = null;
            this.popupNotifier1.OptionsMenu = null;
            this.popupNotifier1.Size = new System.Drawing.Size(400, 130);
            this.popupNotifier1.TitleColor = System.Drawing.Color.ForestGreen;
            this.popupNotifier1.TitleFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.popupNotifier1.TitleText = null;
            this.popupNotifier1.Click += new System.EventHandler(this.popupNotifier1_Click);
            this.popupNotifier1.Close += new System.EventHandler(this.popupNotifier1_Close);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormPopupContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 102);
            this.Name = "FormPopupContainer";
            this.Text = "FormPopupContainer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.FormPopupContainer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PopupNotifier popupNotifier1;
        private System.Windows.Forms.Timer timer1;
    }
}