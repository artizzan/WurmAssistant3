namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager
{
    partial class UcPlayerTriggersController
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonMute = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "player name";
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.Location = new System.Drawing.Point(142, 5);
            this.buttonConfigure.Margin = new System.Windows.Forms.Padding(2);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(116, 29);
            this.buttonConfigure.TabIndex = 2;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            // 
            // buttonMute
            // 
            this.buttonMute.BackgroundImage = global::AldursLab.WurmAssistant3.Core.Properties.Resources.SoundEnabledSmall;
            this.buttonMute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonMute.Location = new System.Drawing.Point(4, 5);
            this.buttonMute.Margin = new System.Windows.Forms.Padding(2);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(34, 29);
            this.buttonMute.TabIndex = 3;
            this.buttonMute.UseVisualStyleBackColor = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackgroundImage = global::AldursLab.WurmAssistant3.Core.Properties.Resources.close_icon;
            this.buttonRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonRemove.Location = new System.Drawing.Point(286, -2);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(16, 17);
            this.buttonRemove.TabIndex = 0;
            this.buttonRemove.UseVisualStyleBackColor = true;
            // 
            // UcPlayerTriggersController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonMute);
            this.Controls.Add(this.buttonConfigure);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonRemove);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UcPlayerTriggersController";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(302, 39);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.Button buttonRemove;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Button buttonConfigure;
        internal System.Windows.Forms.Button buttonMute;
    }
}
