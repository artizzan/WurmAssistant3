namespace AldursLab.WurmAssistant3.Areas.SamplePlugin.CustomViews
{
    partial class SamplePluginForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowSomething = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sample Text";
            // 
            // btnShowSomething
            // 
            this.btnShowSomething.Location = new System.Drawing.Point(78, 55);
            this.btnShowSomething.Name = "btnShowSomething";
            this.btnShowSomething.Size = new System.Drawing.Size(127, 23);
            this.btnShowSomething.TabIndex = 1;
            this.btnShowSomething.Text = "Show something!";
            this.btnShowSomething.UseVisualStyleBackColor = true;
            this.btnShowSomething.Click += new System.EventHandler(this.btnShowSomething_Click);
            // 
            // SamplePluginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnShowSomething);
            this.Controls.Add(this.label1);
            this.Name = "SamplePluginForm";
            this.Text = "SamplePluginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShowSomething;
    }
}