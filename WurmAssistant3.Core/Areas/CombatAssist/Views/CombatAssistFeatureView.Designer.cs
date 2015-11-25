namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Views
{
    partial class CombatAssistFeatureView
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
            this.createLiveSessionBtn = new System.Windows.Forms.Button();
            this.wurmCharacterCbox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.logEntriesBlock = new System.Windows.Forms.TextBox();
            this.parseBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.clearBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // createLiveSessionBtn
            // 
            this.createLiveSessionBtn.Location = new System.Drawing.Point(5, 72);
            this.createLiveSessionBtn.Name = "createLiveSessionBtn";
            this.createLiveSessionBtn.Size = new System.Drawing.Size(160, 23);
            this.createLiveSessionBtn.TabIndex = 0;
            this.createLiveSessionBtn.Text = "Create Live Session";
            this.createLiveSessionBtn.UseVisualStyleBackColor = true;
            this.createLiveSessionBtn.Click += new System.EventHandler(this.createLiveSessionBtn_Click);
            // 
            // wurmCharacterCbox
            // 
            this.wurmCharacterCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wurmCharacterCbox.FormattingEnabled = true;
            this.wurmCharacterCbox.Location = new System.Drawing.Point(5, 45);
            this.wurmCharacterCbox.Name = "wurmCharacterCbox";
            this.wurmCharacterCbox.Size = new System.Drawing.Size(160, 21);
            this.wurmCharacterCbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Game character:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.createLiveSessionBtn);
            this.groupBox1.Controls.Add(this.wurmCharacterCbox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 111);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Live events monitor";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.clearBtn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.parseBtn);
            this.groupBox2.Controls.Add(this.logEntriesBlock);
            this.groupBox2.Location = new System.Drawing.Point(199, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(576, 357);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Combat log parser";
            // 
            // logEntriesBlock
            // 
            this.logEntriesBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logEntriesBlock.Location = new System.Drawing.Point(6, 45);
            this.logEntriesBlock.Multiline = true;
            this.logEntriesBlock.Name = "logEntriesBlock";
            this.logEntriesBlock.Size = new System.Drawing.Size(564, 273);
            this.logEntriesBlock.TabIndex = 0;
            // 
            // parseBtn
            // 
            this.parseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.parseBtn.Location = new System.Drawing.Point(433, 324);
            this.parseBtn.Name = "parseBtn";
            this.parseBtn.Size = new System.Drawing.Size(137, 23);
            this.parseBtn.TabIndex = 1;
            this.parseBtn.Text = "Parse";
            this.parseBtn.UseVisualStyleBackColor = true;
            this.parseBtn.Click += new System.EventHandler(this.parseBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Paste log entries block to parse:";
            // 
            // clearBtn
            // 
            this.clearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearBtn.Location = new System.Drawing.Point(6, 324);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 23);
            this.clearBtn.TabIndex = 3;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // CombatAssistFeatureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 381);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CombatAssistFeatureView";
            this.ShowIcon = false;
            this.Text = "Combat Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CombatAssistFeatureView_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button createLiveSessionBtn;
        private System.Windows.Forms.ComboBox wurmCharacterCbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox logEntriesBlock;
        private System.Windows.Forms.Button parseBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button clearBtn;
    }
}