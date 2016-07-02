namespace AldursLab.WurmAssistant3.Areas.Granger
{
    partial class FormGrangerMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGrangerMain));
            this.checkBoxCapturingEnabled = new System.Windows.Forms.CheckBox();
            this.buttonEditValuePreset = new System.Windows.Forms.Button();
            this.buttonTraitView = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ucGrangerHerdList1 = new UcGrangerHerdList();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ucGrangerTraitView1 = new UcGrangerTraitView();
            this.ucGrangerCreatureList1 = new UcGrangerCreatureList();
            this.buttonHerdView = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonGrangerGeneralOptions = new System.Windows.Forms.Button();
            this.buttonImportExport = new System.Windows.Forms.Button();
            this.comboBoxAdvisor = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAdvisorOptions = new System.Windows.Forms.Button();
            this.comboBoxValuePreset = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonChangePlayers = new System.Windows.Forms.Button();
            this.textBoxCaptureForPlayers = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxCapturingEnabled
            // 
            this.checkBoxCapturingEnabled.AutoSize = true;
            this.checkBoxCapturingEnabled.Location = new System.Drawing.Point(2, 2);
            this.checkBoxCapturingEnabled.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCapturingEnabled.Name = "checkBoxCapturingEnabled";
            this.checkBoxCapturingEnabled.Size = new System.Drawing.Size(157, 17);
            this.checkBoxCapturingEnabled.TabIndex = 1;
            this.checkBoxCapturingEnabled.Text = "monitor events from players:";
            this.checkBoxCapturingEnabled.UseVisualStyleBackColor = true;
            this.checkBoxCapturingEnabled.CheckedChanged += new System.EventHandler(this.checkBoxCapturingEnabled_CheckedChanged);
            // 
            // buttonEditValuePreset
            // 
            this.buttonEditValuePreset.Location = new System.Drawing.Point(258, 20);
            this.buttonEditValuePreset.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEditValuePreset.Name = "buttonEditValuePreset";
            this.buttonEditValuePreset.Size = new System.Drawing.Size(93, 20);
            this.buttonEditValuePreset.TabIndex = 2;
            this.buttonEditValuePreset.Text = "edit presets";
            this.buttonEditValuePreset.UseVisualStyleBackColor = true;
            this.buttonEditValuePreset.Click += new System.EventHandler(this.buttonEditValuePreset_Click);
            // 
            // buttonTraitView
            // 
            this.buttonTraitView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTraitView.Location = new System.Drawing.Point(500, 41);
            this.buttonTraitView.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTraitView.Name = "buttonTraitView";
            this.buttonTraitView.Size = new System.Drawing.Size(72, 23);
            this.buttonTraitView.TabIndex = 4;
            this.buttonTraitView.Text = "trait view";
            this.buttonTraitView.UseVisualStyleBackColor = true;
            this.buttonTraitView.Click += new System.EventHandler(this.buttonTraitView_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ucGrangerHerdList1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 71);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 303F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 320);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // ucGrangerHerdList1
            // 
            this.ucGrangerHerdList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGrangerHerdList1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ucGrangerHerdList1.Location = new System.Drawing.Point(2, 2);
            this.ucGrangerHerdList1.Margin = new System.Windows.Forms.Padding(2);
            this.ucGrangerHerdList1.Name = "ucGrangerHerdList1";
            this.ucGrangerHerdList1.Size = new System.Drawing.Size(108, 316);
            this.ucGrangerHerdList1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(114, 2);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ucGrangerCreatureList1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ucGrangerTraitView1);
            this.splitContainer2.Size = new System.Drawing.Size(456, 316);
            this.splitContainer2.SplitterDistance = 264;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 4;
            // 
            // ucGrangerTraitView1
            // 
            this.ucGrangerTraitView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGrangerTraitView1.Location = new System.Drawing.Point(0, 0);
            this.ucGrangerTraitView1.Margin = new System.Windows.Forms.Padding(2);
            this.ucGrangerTraitView1.Name = "ucGrangerTraitView1";
            this.ucGrangerTraitView1.Size = new System.Drawing.Size(189, 316);
            this.ucGrangerTraitView1.TabIndex = 2;
            // 
            // ucGrangerCreatureList1
            // 
            this.ucGrangerCreatureList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGrangerCreatureList1.Location = new System.Drawing.Point(0, 0);
            this.ucGrangerCreatureList1.Margin = new System.Windows.Forms.Padding(2);
            this.ucGrangerCreatureList1.Name = "ucGrangerCreatureList1";
            this.ucGrangerCreatureList1.SelectedSingleCreature = null;
            this.ucGrangerCreatureList1.Size = new System.Drawing.Size(264, 316);
            this.ucGrangerCreatureList1.TabIndex = 1;
            // 
            // buttonHerdView
            // 
            this.buttonHerdView.Location = new System.Drawing.Point(0, 42);
            this.buttonHerdView.Margin = new System.Windows.Forms.Padding(2);
            this.buttonHerdView.Name = "buttonHerdView";
            this.buttonHerdView.Size = new System.Drawing.Size(72, 23);
            this.buttonHerdView.TabIndex = 9;
            this.buttonHerdView.Text = "herd view";
            this.buttonHerdView.UseVisualStyleBackColor = true;
            this.buttonHerdView.Click += new System.EventHandler(this.buttonHerdView_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(576, 393);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonGrangerGeneralOptions);
            this.panel1.Controls.Add(this.buttonImportExport);
            this.panel1.Controls.Add(this.comboBoxAdvisor);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonAdvisorOptions);
            this.panel1.Controls.Add(this.comboBoxValuePreset);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.buttonChangePlayers);
            this.panel1.Controls.Add(this.textBoxCaptureForPlayers);
            this.panel1.Controls.Add(this.buttonTraitView);
            this.panel1.Controls.Add(this.buttonHerdView);
            this.panel1.Controls.Add(this.buttonEditValuePreset);
            this.panel1.Controls.Add(this.checkBoxCapturingEnabled);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(572, 65);
            this.panel1.TabIndex = 0;
            // 
            // buttonGrangerGeneralOptions
            // 
            this.buttonGrangerGeneralOptions.Location = new System.Drawing.Point(420, 20);
            this.buttonGrangerGeneralOptions.Margin = new System.Windows.Forms.Padding(2);
            this.buttonGrangerGeneralOptions.Name = "buttonGrangerGeneralOptions";
            this.buttonGrangerGeneralOptions.Size = new System.Drawing.Size(55, 41);
            this.buttonGrangerGeneralOptions.TabIndex = 19;
            this.buttonGrangerGeneralOptions.Text = "options";
            this.buttonGrangerGeneralOptions.UseVisualStyleBackColor = true;
            this.buttonGrangerGeneralOptions.Click += new System.EventHandler(this.buttonGrangerGeneralOptions_Click);
            // 
            // buttonImportExport
            // 
            this.buttonImportExport.Location = new System.Drawing.Point(361, 20);
            this.buttonImportExport.Margin = new System.Windows.Forms.Padding(2);
            this.buttonImportExport.Name = "buttonImportExport";
            this.buttonImportExport.Size = new System.Drawing.Size(55, 41);
            this.buttonImportExport.TabIndex = 18;
            this.buttonImportExport.Text = "import export";
            this.buttonImportExport.UseVisualStyleBackColor = true;
            this.buttonImportExport.Click += new System.EventHandler(this.buttonImportExport_Click);
            // 
            // comboBoxAdvisor
            // 
            this.comboBoxAdvisor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAdvisor.FormattingEnabled = true;
            this.comboBoxAdvisor.Location = new System.Drawing.Point(163, 41);
            this.comboBoxAdvisor.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxAdvisor.Name = "comboBoxAdvisor";
            this.comboBoxAdvisor.Size = new System.Drawing.Size(92, 21);
            this.comboBoxAdvisor.TabIndex = 17;
            this.comboBoxAdvisor.TextChanged += new System.EventHandler(this.comboBoxAdvisor_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "advisor:";
            // 
            // buttonAdvisorOptions
            // 
            this.buttonAdvisorOptions.Location = new System.Drawing.Point(258, 42);
            this.buttonAdvisorOptions.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdvisorOptions.Name = "buttonAdvisorOptions";
            this.buttonAdvisorOptions.Size = new System.Drawing.Size(93, 20);
            this.buttonAdvisorOptions.TabIndex = 15;
            this.buttonAdvisorOptions.Text = "advisor options";
            this.buttonAdvisorOptions.UseVisualStyleBackColor = true;
            this.buttonAdvisorOptions.Click += new System.EventHandler(this.buttonAdvisorOptions_Click);
            // 
            // comboBoxValuePreset
            // 
            this.comboBoxValuePreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxValuePreset.FormattingEnabled = true;
            this.comboBoxValuePreset.Location = new System.Drawing.Point(163, 20);
            this.comboBoxValuePreset.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxValuePreset.Name = "comboBoxValuePreset";
            this.comboBoxValuePreset.Size = new System.Drawing.Size(92, 21);
            this.comboBoxValuePreset.TabIndex = 14;
            this.comboBoxValuePreset.TextChanged += new System.EventHandler(this.comboBoxValuePreset_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(92, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "value preset:";
            // 
            // buttonChangePlayers
            // 
            this.buttonChangePlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChangePlayers.Location = new System.Drawing.Point(481, 0);
            this.buttonChangePlayers.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChangePlayers.Name = "buttonChangePlayers";
            this.buttonChangePlayers.Size = new System.Drawing.Size(88, 20);
            this.buttonChangePlayers.TabIndex = 12;
            this.buttonChangePlayers.Text = "change";
            this.buttonChangePlayers.UseVisualStyleBackColor = true;
            this.buttonChangePlayers.Click += new System.EventHandler(this.buttonChangePlayers_Click);
            // 
            // textBoxCaptureForPlayers
            // 
            this.textBoxCaptureForPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCaptureForPlayers.Location = new System.Drawing.Point(164, 1);
            this.textBoxCaptureForPlayers.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCaptureForPlayers.Name = "textBoxCaptureForPlayers";
            this.textBoxCaptureForPlayers.ReadOnly = true;
            this.textBoxCaptureForPlayers.Size = new System.Drawing.Size(314, 20);
            this.textBoxCaptureForPlayers.TabIndex = 11;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormGrangerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 393);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(592, 400);
            this.Name = "FormGrangerMain";
            this.Text = "Granger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGrangerMain_FormClosing);
            this.Load += new System.EventHandler(this.FormGrangerMain_Load);
            this.Resize += new System.EventHandler(this.FormGrangerMain_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxCapturingEnabled;
        private System.Windows.Forms.Button buttonEditValuePreset;
        private System.Windows.Forms.Button buttonTraitView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonHerdView;
        private UcGrangerHerdList ucGrangerHerdList1;
        private UcGrangerCreatureList ucGrangerCreatureList1;
        private UcGrangerTraitView ucGrangerTraitView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonChangePlayers;
        private System.Windows.Forms.TextBox textBoxCaptureForPlayers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAdvisorOptions;
        private System.Windows.Forms.Button buttonImportExport;
        private System.Windows.Forms.ComboBox comboBoxAdvisor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxValuePreset;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonGrangerGeneralOptions;
    }
}