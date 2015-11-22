namespace AldursLab.WurmAssistant3.Core.Areas.CraftingAssistant.Views
{
    partial class CraftingAssistantView
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
            this.createWidgetBtn = new System.Windows.Forms.Button();
            this.gameCharactersCmb = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // createWidgetBtn
            // 
            this.createWidgetBtn.Location = new System.Drawing.Point(12, 56);
            this.createWidgetBtn.Name = "createWidgetBtn";
            this.createWidgetBtn.Size = new System.Drawing.Size(130, 23);
            this.createWidgetBtn.TabIndex = 1;
            this.createWidgetBtn.Text = "Create widget";
            this.createWidgetBtn.UseVisualStyleBackColor = true;
            this.createWidgetBtn.Click += new System.EventHandler(this.createWidgetBtn_Click);
            // 
            // gameCharactersCmb
            // 
            this.gameCharactersCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gameCharactersCmb.FormattingEnabled = true;
            this.gameCharactersCmb.Location = new System.Drawing.Point(12, 29);
            this.gameCharactersCmb.Name = "gameCharactersCmb";
            this.gameCharactersCmb.Size = new System.Drawing.Size(183, 21);
            this.gameCharactersCmb.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Choose game character:";
            // 
            // CraftingAssistantView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 112);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gameCharactersCmb);
            this.Controls.Add(this.createWidgetBtn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CraftingAssistantView";
            this.ShowIcon = false;
            this.Text = "Crafting Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CraftingAssistantView_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button createWidgetBtn;
        private System.Windows.Forms.ComboBox gameCharactersCmb;
        private System.Windows.Forms.Label label2;
    }
}