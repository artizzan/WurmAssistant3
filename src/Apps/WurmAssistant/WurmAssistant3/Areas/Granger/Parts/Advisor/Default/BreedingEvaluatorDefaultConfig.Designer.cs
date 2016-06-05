using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.Advisor.Default
{
    partial class BreedingEvaluatorDefaultConfig
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxSkipNotInMood = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipPregnant = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipGaveBirthInLast24h = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipCreaturesInOtherHerds = new System.Windows.Forms.CheckBox();
            this.checkBoxIncludePotentialValue = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxDiscardWithBadTraits = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownBadTraitWeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPotValBadWeight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPotValGoodWeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownUniqueTraitWeight = new System.Windows.Forms.NumericUpDown();
            this.checkBoxPreferUniqueTraits = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownInbreedPenaltyWeight = new System.Windows.Forms.NumericUpDown();
            this.checkBoxDiscardAllCausingInbreed = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxSkipPaired = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxExcludeFoals = new System.Windows.Forms.CheckBox();
            this.checkBoxExcludeYoung = new System.Windows.Forms.CheckBox();
            this.checkBoxExcludeAdolescent = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxExcludeExactAge = new System.Windows.Forms.CheckBox();
            this.timeSpanInputExcludeExactAge = new TimeSpanInput();
            this.checkBoxKeepComparingSelected = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBoxIgnoreDead = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnoreSold = new System.Windows.Forms.CheckBox();
            this.objectListViewColorWeights = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnColorType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnColorWeight = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.labelAutowrap1 = new LabelAutowrap();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBadTraitWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPotValBadWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPotValGoodWeight)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUniqueTraitWeight)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInbreedPenaltyWeight)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewColorWeights)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(666, 354);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(78, 29);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(748, 354);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(77, 29);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipNotInMood
            // 
            this.checkBoxSkipNotInMood.AutoSize = true;
            this.checkBoxSkipNotInMood.Location = new System.Drawing.Point(4, 17);
            this.checkBoxSkipNotInMood.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipNotInMood.Name = "checkBoxSkipNotInMood";
            this.checkBoxSkipNotInMood.Size = new System.Drawing.Size(162, 17);
            this.checkBoxSkipNotInMood.TabIndex = 2;
            this.checkBoxSkipNotInMood.Text = "Skip \"not in mood\" creatures";
            this.checkBoxSkipNotInMood.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipPregnant
            // 
            this.checkBoxSkipPregnant.AutoSize = true;
            this.checkBoxSkipPregnant.Location = new System.Drawing.Point(4, 39);
            this.checkBoxSkipPregnant.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipPregnant.Name = "checkBoxSkipPregnant";
            this.checkBoxSkipPregnant.Size = new System.Drawing.Size(139, 17);
            this.checkBoxSkipPregnant.TabIndex = 3;
            this.checkBoxSkipPregnant.Text = "Skip pregnant creatures";
            this.checkBoxSkipPregnant.UseVisualStyleBackColor = true;
            this.checkBoxSkipPregnant.CheckedChanged += new System.EventHandler(this.checkBoxSkipPregnant_CheckedChanged);
            // 
            // checkBoxSkipGaveBirthInLast24h
            // 
            this.checkBoxSkipGaveBirthInLast24h.AutoSize = true;
            this.checkBoxSkipGaveBirthInLast24h.Location = new System.Drawing.Point(4, 61);
            this.checkBoxSkipGaveBirthInLast24h.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipGaveBirthInLast24h.Name = "checkBoxSkipGaveBirthInLast24h";
            this.checkBoxSkipGaveBirthInLast24h.Size = new System.Drawing.Size(297, 17);
            this.checkBoxSkipGaveBirthInLast24h.TabIndex = 4;
            this.checkBoxSkipGaveBirthInLast24h.Text = "Skip creatures, that gave birth within last 24h (inaccurate)";
            this.checkBoxSkipGaveBirthInLast24h.UseVisualStyleBackColor = true;
            this.checkBoxSkipGaveBirthInLast24h.CheckedChanged += new System.EventHandler(this.checkBoxSkipGaveBirthInLast24h_CheckedChanged);
            // 
            // checkBoxSkipCreaturesInOtherHerds
            // 
            this.checkBoxSkipCreaturesInOtherHerds.AutoSize = true;
            this.checkBoxSkipCreaturesInOtherHerds.Location = new System.Drawing.Point(4, 83);
            this.checkBoxSkipCreaturesInOtherHerds.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipCreaturesInOtherHerds.Name = "checkBoxSkipCreaturesInOtherHerds";
            this.checkBoxSkipCreaturesInOtherHerds.Size = new System.Drawing.Size(277, 17);
            this.checkBoxSkipCreaturesInOtherHerds.TabIndex = 5;
            this.checkBoxSkipCreaturesInOtherHerds.Text = "Skip creatures in other herds than evaluated creature";
            this.checkBoxSkipCreaturesInOtherHerds.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludePotentialValue
            // 
            this.checkBoxIncludePotentialValue.AutoSize = true;
            this.checkBoxIncludePotentialValue.Location = new System.Drawing.Point(4, 61);
            this.checkBoxIncludePotentialValue.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxIncludePotentialValue.Name = "checkBoxIncludePotentialValue";
            this.checkBoxIncludePotentialValue.Size = new System.Drawing.Size(175, 17);
            this.checkBoxIncludePotentialValue.TabIndex = 6;
            this.checkBoxIncludePotentialValue.Text = "Include potential creature value";
            this.checkBoxIncludePotentialValue.UseVisualStyleBackColor = true;
            this.checkBoxIncludePotentialValue.CheckedChanged += new System.EventHandler(this.checkBoxIncludePotentialValue_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxDiscardWithBadTraits);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownBadTraitWeight);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownPotValBadWeight);
            this.groupBox1.Controls.Add(this.numericUpDownPotValGoodWeight);
            this.groupBox1.Controls.Add(this.checkBoxIncludePotentialValue);
            this.groupBox1.Location = new System.Drawing.Point(315, 84);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(284, 137);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Creature value";
            // 
            // checkBoxDiscardWithBadTraits
            // 
            this.checkBoxDiscardWithBadTraits.AutoSize = true;
            this.checkBoxDiscardWithBadTraits.Location = new System.Drawing.Point(4, 39);
            this.checkBoxDiscardWithBadTraits.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDiscardWithBadTraits.Name = "checkBoxDiscardWithBadTraits";
            this.checkBoxDiscardWithBadTraits.Size = new System.Drawing.Size(210, 17);
            this.checkBoxDiscardWithBadTraits.TabIndex = 6;
            this.checkBoxDiscardWithBadTraits.Text = "Discard all creatures with any bad traits";
            this.checkBoxDiscardWithBadTraits.UseVisualStyleBackColor = true;
            this.checkBoxDiscardWithBadTraits.CheckedChanged += new System.EventHandler(this.checkBoxDiscardWithBadTraits_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "bad trait weight:";
            // 
            // numericUpDownBadTraitWeight
            // 
            this.numericUpDownBadTraitWeight.DecimalPlaces = 2;
            this.numericUpDownBadTraitWeight.Location = new System.Drawing.Point(90, 16);
            this.numericUpDownBadTraitWeight.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBadTraitWeight.Name = "numericUpDownBadTraitWeight";
            this.numericUpDownBadTraitWeight.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownBadTraitWeight.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 108);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Potential bad traits weight:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Potential good traits weight:";
            // 
            // numericUpDownPotValBadWeight
            // 
            this.numericUpDownPotValBadWeight.DecimalPlaces = 2;
            this.numericUpDownPotValBadWeight.Location = new System.Drawing.Point(146, 106);
            this.numericUpDownPotValBadWeight.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPotValBadWeight.Name = "numericUpDownPotValBadWeight";
            this.numericUpDownPotValBadWeight.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownPotValBadWeight.TabIndex = 8;
            // 
            // numericUpDownPotValGoodWeight
            // 
            this.numericUpDownPotValGoodWeight.DecimalPlaces = 2;
            this.numericUpDownPotValGoodWeight.Location = new System.Drawing.Point(146, 83);
            this.numericUpDownPotValGoodWeight.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPotValGoodWeight.Name = "numericUpDownPotValGoodWeight";
            this.numericUpDownPotValGoodWeight.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownPotValGoodWeight.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDownUniqueTraitWeight);
            this.groupBox2.Controls.Add(this.checkBoxPreferUniqueTraits);
            this.groupBox2.Location = new System.Drawing.Point(315, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(284, 70);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trait matching";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Unique trait weight:";
            // 
            // numericUpDownUniqueTraitWeight
            // 
            this.numericUpDownUniqueTraitWeight.DecimalPlaces = 2;
            this.numericUpDownUniqueTraitWeight.Location = new System.Drawing.Point(110, 39);
            this.numericUpDownUniqueTraitWeight.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownUniqueTraitWeight.Name = "numericUpDownUniqueTraitWeight";
            this.numericUpDownUniqueTraitWeight.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownUniqueTraitWeight.TabIndex = 7;
            // 
            // checkBoxPreferUniqueTraits
            // 
            this.checkBoxPreferUniqueTraits.AutoSize = true;
            this.checkBoxPreferUniqueTraits.Location = new System.Drawing.Point(4, 17);
            this.checkBoxPreferUniqueTraits.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxPreferUniqueTraits.Name = "checkBoxPreferUniqueTraits";
            this.checkBoxPreferUniqueTraits.Size = new System.Drawing.Size(114, 17);
            this.checkBoxPreferUniqueTraits.TabIndex = 6;
            this.checkBoxPreferUniqueTraits.Text = "Prefer unique traits";
            this.checkBoxPreferUniqueTraits.UseVisualStyleBackColor = true;
            this.checkBoxPreferUniqueTraits.CheckedChanged += new System.EventHandler(this.checkBoxPreferUniqueTraits_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.numericUpDownInbreedPenaltyWeight);
            this.groupBox4.Controls.Add(this.checkBoxDiscardAllCausingInbreed);
            this.groupBox4.Location = new System.Drawing.Point(315, 227);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(284, 60);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Inbreeding (simulated as 1 extra bad trait on offspring)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Inbreeding \"fictional\" bad trait weight:";
            // 
            // numericUpDownInbreedPenaltyWeight
            // 
            this.numericUpDownInbreedPenaltyWeight.DecimalPlaces = 2;
            this.numericUpDownInbreedPenaltyWeight.Location = new System.Drawing.Point(188, 19);
            this.numericUpDownInbreedPenaltyWeight.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownInbreedPenaltyWeight.Name = "numericUpDownInbreedPenaltyWeight";
            this.numericUpDownInbreedPenaltyWeight.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownInbreedPenaltyWeight.TabIndex = 7;
            // 
            // checkBoxDiscardAllCausingInbreed
            // 
            this.checkBoxDiscardAllCausingInbreed.AutoSize = true;
            this.checkBoxDiscardAllCausingInbreed.Location = new System.Drawing.Point(4, 41);
            this.checkBoxDiscardAllCausingInbreed.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDiscardAllCausingInbreed.Name = "checkBoxDiscardAllCausingInbreed";
            this.checkBoxDiscardAllCausingInbreed.Size = new System.Drawing.Size(258, 17);
            this.checkBoxDiscardAllCausingInbreed.TabIndex = 6;
            this.checkBoxDiscardAllCausingInbreed.Text = "Discard all creatures that would cause inbreeding";
            this.checkBoxDiscardAllCausingInbreed.UseVisualStyleBackColor = true;
            this.checkBoxDiscardAllCausingInbreed.CheckedChanged += new System.EventHandler(this.checkBoxDiscardAllCausingInbreed_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxSkipPaired);
            this.groupBox3.Controls.Add(this.checkBoxSkipNotInMood);
            this.groupBox3.Controls.Add(this.checkBoxSkipPregnant);
            this.groupBox3.Controls.Add(this.checkBoxSkipGaveBirthInLast24h);
            this.groupBox3.Controls.Add(this.checkBoxSkipCreaturesInOtherHerds);
            this.groupBox3.Location = new System.Drawing.Point(9, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(302, 131);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Exclude options";
            // 
            // checkBoxSkipPaired
            // 
            this.checkBoxSkipPaired.AutoSize = true;
            this.checkBoxSkipPaired.Location = new System.Drawing.Point(4, 105);
            this.checkBoxSkipPaired.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSkipPaired.Name = "checkBoxSkipPaired";
            this.checkBoxSkipPaired.Size = new System.Drawing.Size(146, 17);
            this.checkBoxSkipPaired.TabIndex = 6;
            this.checkBoxSkipPaired.Text = "Skip any paired creatures";
            this.checkBoxSkipPaired.UseVisualStyleBackColor = true;
            this.checkBoxSkipPaired.CheckedChanged += new System.EventHandler(this.checkBoxSkipPaired_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(9, 354);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 29);
            this.button1.TabIndex = 15;
            this.button1.Text = "More help";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 30000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ShowAlways = true;
            // 
            // checkBoxExcludeFoals
            // 
            this.checkBoxExcludeFoals.AutoSize = true;
            this.checkBoxExcludeFoals.Location = new System.Drawing.Point(4, 17);
            this.checkBoxExcludeFoals.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxExcludeFoals.Name = "checkBoxExcludeFoals";
            this.checkBoxExcludeFoals.Size = new System.Drawing.Size(205, 17);
            this.checkBoxExcludeFoals.TabIndex = 7;
            this.checkBoxExcludeFoals.Text = "Foals (young foal and adolescent foal)";
            this.checkBoxExcludeFoals.UseVisualStyleBackColor = true;
            this.checkBoxExcludeFoals.CheckedChanged += new System.EventHandler(this.checkBoxExcludeFoals_CheckedChanged);
            // 
            // checkBoxExcludeYoung
            // 
            this.checkBoxExcludeYoung.AutoSize = true;
            this.checkBoxExcludeYoung.Location = new System.Drawing.Point(4, 39);
            this.checkBoxExcludeYoung.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxExcludeYoung.Name = "checkBoxExcludeYoung";
            this.checkBoxExcludeYoung.Size = new System.Drawing.Size(95, 17);
            this.checkBoxExcludeYoung.TabIndex = 8;
            this.checkBoxExcludeYoung.Text = "Regular young";
            this.checkBoxExcludeYoung.UseVisualStyleBackColor = true;
            this.checkBoxExcludeYoung.CheckedChanged += new System.EventHandler(this.checkBoxExcludeYoung_CheckedChanged);
            // 
            // checkBoxExcludeAdolescent
            // 
            this.checkBoxExcludeAdolescent.AutoSize = true;
            this.checkBoxExcludeAdolescent.Location = new System.Drawing.Point(4, 61);
            this.checkBoxExcludeAdolescent.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxExcludeAdolescent.Name = "checkBoxExcludeAdolescent";
            this.checkBoxExcludeAdolescent.Size = new System.Drawing.Size(118, 17);
            this.checkBoxExcludeAdolescent.TabIndex = 9;
            this.checkBoxExcludeAdolescent.Text = "Regular adolescent";
            this.checkBoxExcludeAdolescent.UseVisualStyleBackColor = true;
            this.checkBoxExcludeAdolescent.CheckedChanged += new System.EventHandler(this.checkBoxExcludeAdolescent_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxExcludeExactAge);
            this.groupBox5.Controls.Add(this.timeSpanInputExcludeExactAge);
            this.groupBox5.Controls.Add(this.checkBoxKeepComparingSelected);
            this.groupBox5.Controls.Add(this.checkBoxExcludeAdolescent);
            this.groupBox5.Controls.Add(this.checkBoxExcludeFoals);
            this.groupBox5.Controls.Add(this.checkBoxExcludeYoung);
            this.groupBox5.Location = new System.Drawing.Point(9, 145);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(302, 206);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Age exclude options";
            this.groupBox5.Enter += new System.EventHandler(this.groupBox5_Enter);
            // 
            // checkBoxExcludeExactAge
            // 
            this.checkBoxExcludeExactAge.AutoSize = true;
            this.checkBoxExcludeExactAge.Location = new System.Drawing.Point(4, 112);
            this.checkBoxExcludeExactAge.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxExcludeExactAge.Name = "checkBoxExcludeExactAge";
            this.checkBoxExcludeExactAge.Size = new System.Drawing.Size(278, 30);
            this.checkBoxExcludeExactAge.TabIndex = 12;
            this.checkBoxExcludeExactAge.Text = "Exclude creature younger than:\r\n(this works only for creatures  that have birth d" +
    "ate set)";
            this.checkBoxExcludeExactAge.UseVisualStyleBackColor = true;
            this.checkBoxExcludeExactAge.CheckedChanged += new System.EventHandler(this.checkBoxExcludeExactAge_CheckedChanged);
            // 
            // timeSpanInputExcludeExactAge
            // 
            this.timeSpanInputExcludeExactAge.Location = new System.Drawing.Point(4, 146);
            this.timeSpanInputExcludeExactAge.Margin = new System.Windows.Forms.Padding(2);
            this.timeSpanInputExcludeExactAge.Name = "timeSpanInputExcludeExactAge";
            this.timeSpanInputExcludeExactAge.Size = new System.Drawing.Size(231, 45);
            this.timeSpanInputExcludeExactAge.TabIndex = 11;
            this.timeSpanInputExcludeExactAge.Value = System.TimeSpan.Parse("00:00:00");
            // 
            // checkBoxKeepComparingSelected
            // 
            this.checkBoxKeepComparingSelected.AutoSize = true;
            this.checkBoxKeepComparingSelected.Location = new System.Drawing.Point(4, 83);
            this.checkBoxKeepComparingSelected.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxKeepComparingSelected.Name = "checkBoxKeepComparingSelected";
            this.checkBoxKeepComparingSelected.Size = new System.Drawing.Size(206, 17);
            this.checkBoxKeepComparingSelected.TabIndex = 10;
            this.checkBoxKeepComparingSelected.Text = "Keep comparing the selected creature";
            this.checkBoxKeepComparingSelected.UseVisualStyleBackColor = true;
            this.checkBoxKeepComparingSelected.CheckedChanged += new System.EventHandler(this.checkBoxKeepComparingSelected_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBoxIgnoreDead);
            this.groupBox6.Controls.Add(this.checkBoxIgnoreSold);
            this.groupBox6.Location = new System.Drawing.Point(315, 291);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(284, 60);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Exclude tags";
            // 
            // checkBoxIgnoreDead
            // 
            this.checkBoxIgnoreDead.AutoSize = true;
            this.checkBoxIgnoreDead.Location = new System.Drawing.Point(4, 39);
            this.checkBoxIgnoreDead.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxIgnoreDead.Name = "checkBoxIgnoreDead";
            this.checkBoxIgnoreDead.Size = new System.Drawing.Size(84, 17);
            this.checkBoxIgnoreDead.TabIndex = 8;
            this.checkBoxIgnoreDead.Text = "Skip \"dead\"";
            this.checkBoxIgnoreDead.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreSold
            // 
            this.checkBoxIgnoreSold.AutoSize = true;
            this.checkBoxIgnoreSold.Location = new System.Drawing.Point(4, 17);
            this.checkBoxIgnoreSold.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxIgnoreSold.Name = "checkBoxIgnoreSold";
            this.checkBoxIgnoreSold.Size = new System.Drawing.Size(79, 17);
            this.checkBoxIgnoreSold.TabIndex = 7;
            this.checkBoxIgnoreSold.Text = "Skip \"sold\"";
            this.checkBoxIgnoreSold.UseVisualStyleBackColor = true;
            // 
            // objectListViewColorWeights
            // 
            this.objectListViewColorWeights.AllColumns.Add(this.olvColumnColorType);
            this.objectListViewColorWeights.AllColumns.Add(this.olvColumnColorWeight);
            this.objectListViewColorWeights.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.objectListViewColorWeights.CellEditTabChangesRows = true;
            this.objectListViewColorWeights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnColorType,
            this.olvColumnColorWeight});
            this.objectListViewColorWeights.GridLines = true;
            this.objectListViewColorWeights.HasCollapsibleGroups = false;
            this.objectListViewColorWeights.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListViewColorWeights.Location = new System.Drawing.Point(8, 21);
            this.objectListViewColorWeights.MultiSelect = false;
            this.objectListViewColorWeights.Name = "objectListViewColorWeights";
            this.objectListViewColorWeights.ShowFilterMenuOnRightClick = false;
            this.objectListViewColorWeights.ShowGroups = false;
            this.objectListViewColorWeights.Size = new System.Drawing.Size(204, 235);
            this.objectListViewColorWeights.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.objectListViewColorWeights.TabIndex = 18;
            this.objectListViewColorWeights.UseCellFormatEvents = true;
            this.objectListViewColorWeights.UseCompatibleStateImageBehavior = false;
            this.objectListViewColorWeights.View = System.Windows.Forms.View.Details;
            this.objectListViewColorWeights.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.objectListViewColorWeights_FormatCell);
            // 
            // olvColumnColorType
            // 
            this.olvColumnColorType.AspectName = "Color";
            this.olvColumnColorType.Groupable = false;
            this.olvColumnColorType.Hideable = false;
            this.olvColumnColorType.IsEditable = false;
            this.olvColumnColorType.Sortable = false;
            this.olvColumnColorType.Text = "Color";
            this.olvColumnColorType.UseFiltering = false;
            this.olvColumnColorType.Width = 120;
            // 
            // olvColumnColorWeight
            // 
            this.olvColumnColorWeight.AspectName = "Weight";
            this.olvColumnColorWeight.Hideable = false;
            this.olvColumnColorWeight.Sortable = false;
            this.olvColumnColorWeight.Text = "Weight";
            this.olvColumnColorWeight.UseFiltering = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.labelAutowrap1);
            this.groupBox7.Controls.Add(this.objectListViewColorWeights);
            this.groupBox7.Location = new System.Drawing.Point(604, 10);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox7.Size = new System.Drawing.Size(220, 341);
            this.groupBox7.TabIndex = 19;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Value Weight for Colors";
            // 
            // labelAutowrap1
            // 
            this.labelAutowrap1.Location = new System.Drawing.Point(8, 259);
            this.labelAutowrap1.Name = "labelAutowrap1";
            this.labelAutowrap1.Size = new System.Drawing.Size(204, 39);
            this.labelAutowrap1.TabIndex = 19;
            this.labelAutowrap1.Text = "Total breeding value will be multiplied by an average of both creatures color weight" +
    "s. ex: (1.5 + 1.2 ) / 2 = 1.35";
            // 
            // BreedingEvaluatorDefaultConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 393);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BreedingEvaluatorDefaultConfig";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Breeding advisor options (evaluator: default)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBadTraitWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPotValBadWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPotValGoodWeight)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUniqueTraitWeight)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInbreedPenaltyWeight)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewColorWeights)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxSkipNotInMood;
        private System.Windows.Forms.CheckBox checkBoxSkipPregnant;
        private System.Windows.Forms.CheckBox checkBoxSkipGaveBirthInLast24h;
        private System.Windows.Forms.CheckBox checkBoxSkipCreaturesInOtherHerds;
        private System.Windows.Forms.CheckBox checkBoxIncludePotentialValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownPotValBadWeight;
        private System.Windows.Forms.NumericUpDown numericUpDownPotValGoodWeight;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownUniqueTraitWeight;
        private System.Windows.Forms.CheckBox checkBoxPreferUniqueTraits;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownBadTraitWeight;
        private System.Windows.Forms.CheckBox checkBoxDiscardWithBadTraits;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownInbreedPenaltyWeight;
        private System.Windows.Forms.CheckBox checkBoxDiscardAllCausingInbreed;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxSkipPaired;
        private System.Windows.Forms.CheckBox checkBoxExcludeFoals;
        private System.Windows.Forms.CheckBox checkBoxExcludeYoung;
        private System.Windows.Forms.CheckBox checkBoxExcludeAdolescent;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox checkBoxIgnoreSold;
        private System.Windows.Forms.CheckBox checkBoxIgnoreDead;
        private System.Windows.Forms.CheckBox checkBoxKeepComparingSelected;
        private BrightIdeasSoftware.ObjectListView objectListViewColorWeights;
        private System.Windows.Forms.GroupBox groupBox7;
        private BrightIdeasSoftware.OLVColumn olvColumnColorType;
        private BrightIdeasSoftware.OLVColumn olvColumnColorWeight;
        private LabelAutowrap labelAutowrap1;
        private System.Windows.Forms.CheckBox checkBoxExcludeExactAge;
        private TimeSpanInput timeSpanInputExcludeExactAge;
    }
}