namespace AldursLab.WurmAssistant3.Areas.Persistence.Views
{
    partial class DeserializationErrorResolverForm
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
            this.ErrorTextTb = new System.Windows.Forms.TextBox();
            this.IgnoreBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ErrorTextTb
            // 
            this.ErrorTextTb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorTextTb.Location = new System.Drawing.Point(12, 12);
            this.ErrorTextTb.Multiline = true;
            this.ErrorTextTb.Name = "ErrorTextTb";
            this.ErrorTextTb.ReadOnly = true;
            this.ErrorTextTb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ErrorTextTb.Size = new System.Drawing.Size(520, 207);
            this.ErrorTextTb.TabIndex = 0;
            this.ErrorTextTb.TabStop = false;
            // 
            // IgnoreBtn
            // 
            this.IgnoreBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IgnoreBtn.Location = new System.Drawing.Point(12, 227);
            this.IgnoreBtn.Name = "IgnoreBtn";
            this.IgnoreBtn.Size = new System.Drawing.Size(224, 105);
            this.IgnoreBtn.TabIndex = 1;
            this.IgnoreBtn.TabStop = false;
            this.IgnoreBtn.Text = "Ignore this error.\r\n\r\nWARNING\r\nSome settings or data may be lost or will return t" +
    "o default. \r\nApp may not work correctly.";
            this.IgnoreBtn.UseVisualStyleBackColor = true;
            this.IgnoreBtn.Click += new System.EventHandler(this.IgnoreBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ExitBtn.Location = new System.Drawing.Point(308, 228);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(224, 105);
            this.ExitBtn.TabIndex = 2;
            this.ExitBtn.Text = "Stop the app.\r\n(recommended)\r\n\r\nContact support to resolve this error in a safe w" +
    "ay.";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // DeserializationErrorResolverView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 344);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.IgnoreBtn);
            this.Controls.Add(this.ErrorTextTb);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "DeserializationErrorResolverForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "There was an error during loading of some data.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ErrorTextTb;
        private System.Windows.Forms.Button IgnoreBtn;
        private System.Windows.Forms.Button ExitBtn;
    }
}