namespace AldursLab.WurmAssistant3.Areas.Config.Transients
{
    partial class SettingsEditForm
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
            this.firstTimeSetupAgain = new System.Windows.Forms.CheckBox();
            this.cleanWurmApiCaches = new System.Windows.Forms.CheckBox();
            this.validateConfigButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // firstTimeSetupAgain
            // 
            this.firstTimeSetupAgain.AutoSize = true;
            this.firstTimeSetupAgain.Location = new System.Drawing.Point(12, 12);
            this.firstTimeSetupAgain.Name = "firstTimeSetupAgain";
            this.firstTimeSetupAgain.Size = new System.Drawing.Size(248, 17);
            this.firstTimeSetupAgain.TabIndex = 0;
            this.firstTimeSetupAgain.Text = "Run \'first time setup\' again (requires app restart)";
            this.firstTimeSetupAgain.UseVisualStyleBackColor = true;
            this.firstTimeSetupAgain.CheckedChanged += new System.EventHandler(this.firstTimeSetupAgain_CheckedChanged);
            // 
            // cleanWurmApiCaches
            // 
            this.cleanWurmApiCaches.AutoSize = true;
            this.cleanWurmApiCaches.Location = new System.Drawing.Point(12, 35);
            this.cleanWurmApiCaches.Name = "cleanWurmApiCaches";
            this.cleanWurmApiCaches.Size = new System.Drawing.Size(246, 17);
            this.cleanWurmApiCaches.TabIndex = 1;
            this.cleanWurmApiCaches.Text = "Clear all WurmApi caches (requires app restart)";
            this.cleanWurmApiCaches.UseVisualStyleBackColor = true;
            this.cleanWurmApiCaches.CheckedChanged += new System.EventHandler(this.cleanWurmApiCaches_CheckedChanged);
            // 
            // validateConfigButton
            // 
            this.validateConfigButton.Location = new System.Drawing.Point(12, 97);
            this.validateConfigButton.Name = "validateConfigButton";
            this.validateConfigButton.Size = new System.Drawing.Size(246, 23);
            this.validateConfigButton.TabIndex = 3;
            this.validateConfigButton.Text = "Validate Wurm game client config";
            this.validateConfigButton.UseVisualStyleBackColor = true;
            this.validateConfigButton.Click += new System.EventHandler(this.validateConfigButton_Click);
            // 
            // SettingsEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 139);
            this.Controls.Add(this.validateConfigButton);
            this.Controls.Add(this.cleanWurmApiCaches);
            this.Controls.Add(this.firstTimeSetupAgain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsEditForm";
            this.ShowIcon = false;
            this.Text = "Wurm Assistant Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox firstTimeSetupAgain;
        private System.Windows.Forms.CheckBox cleanWurmApiCaches;
        private System.Windows.Forms.Button validateConfigButton;
    }
}