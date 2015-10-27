namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.HorseEdit
{
    partial class FormHorseViewEdit
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
            this.checkedListBoxTraits = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxGender = new System.Windows.Forms.GroupBox();
            this.radioButtonFemale = new System.Windows.Forms.RadioButton();
            this.radioButtonMale = new System.Windows.Forms.RadioButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dateTimePickerBred = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerGroomed = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerPregnant = new System.Windows.Forms.DateTimePicker();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxDiseased = new System.Windows.Forms.CheckBox();
            this.checkBoxDead = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dateTimePickerBirthDate = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.buttonApplyTraitsFromPasteText = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxPasteUpdate = new System.Windows.Forms.TextBox();
            this.checkBoxSold = new System.Windows.Forms.CheckBox();
            this.textBoxBrandedFor = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxMate = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxAge = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxColor = new System.Windows.Forms.ComboBox();
            this.checkBoxEpic = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.textBoxCaredForBy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownAHskill = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxFather = new System.Windows.Forms.ComboBox();
            this.comboBoxMother = new System.Windows.Forms.ComboBox();
            this.labelWarn = new System.Windows.Forms.Label();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.groupBoxGender.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAHskill)).BeginInit();
            this.SuspendLayout();
            // 
            // checkedListBoxTraits
            // 
            this.checkedListBoxTraits.FormattingEnabled = true;
            this.checkedListBoxTraits.Location = new System.Drawing.Point(4, 18);
            this.checkedListBoxTraits.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBoxTraits.Name = "checkedListBoxTraits";
            this.checkedListBoxTraits.Size = new System.Drawing.Size(222, 379);
            this.checkedListBoxTraits.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Traits:";
            // 
            // groupBoxGender
            // 
            this.groupBoxGender.Controls.Add(this.radioButtonFemale);
            this.groupBoxGender.Controls.Add(this.radioButtonMale);
            this.groupBoxGender.Location = new System.Drawing.Point(442, 2);
            this.groupBoxGender.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxGender.Name = "groupBoxGender";
            this.groupBoxGender.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxGender.Size = new System.Drawing.Size(126, 72);
            this.groupBoxGender.TabIndex = 2;
            this.groupBoxGender.TabStop = false;
            this.groupBoxGender.Text = "Gender";
            // 
            // radioButtonFemale
            // 
            this.radioButtonFemale.AutoSize = true;
            this.radioButtonFemale.Checked = true;
            this.radioButtonFemale.Location = new System.Drawing.Point(4, 41);
            this.radioButtonFemale.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonFemale.Name = "radioButtonFemale";
            this.radioButtonFemale.Size = new System.Drawing.Size(56, 17);
            this.radioButtonFemale.TabIndex = 2;
            this.radioButtonFemale.TabStop = true;
            this.radioButtonFemale.Text = "female";
            this.radioButtonFemale.UseVisualStyleBackColor = true;
            this.radioButtonFemale.CheckedChanged += new System.EventHandler(this.radioButtonFemale_CheckedChanged);
            // 
            // radioButtonMale
            // 
            this.radioButtonMale.AutoSize = true;
            this.radioButtonMale.Location = new System.Drawing.Point(4, 20);
            this.radioButtonMale.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonMale.Name = "radioButtonMale";
            this.radioButtonMale.Size = new System.Drawing.Size(47, 17);
            this.radioButtonMale.TabIndex = 1;
            this.radioButtonMale.Text = "male";
            this.radioButtonMale.UseVisualStyleBackColor = true;
            this.radioButtonMale.CheckedChanged += new System.EventHandler(this.radioButtonMale_CheckedChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(232, 18);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxName.MaxLength = 50;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(198, 20);
            this.textBoxName.TabIndex = 0;
            this.textBoxName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(230, 38);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mother";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(230, 76);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Father";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(440, 176);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Not in mood until";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(440, 213);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Last groomed on";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(440, 249);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Will give birth on";
            // 
            // dateTimePickerBred
            // 
            this.dateTimePickerBred.CustomFormat = "yyyy-MM-dd   HH:mm";
            this.dateTimePickerBred.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerBred.Location = new System.Drawing.Point(442, 193);
            this.dateTimePickerBred.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerBred.Name = "dateTimePickerBred";
            this.dateTimePickerBred.Size = new System.Drawing.Size(127, 20);
            this.dateTimePickerBred.TabIndex = 5;
            // 
            // dateTimePickerGroomed
            // 
            this.dateTimePickerGroomed.CustomFormat = "yyyy-MM-dd   HH:mm";
            this.dateTimePickerGroomed.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerGroomed.Location = new System.Drawing.Point(442, 229);
            this.dateTimePickerGroomed.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerGroomed.Name = "dateTimePickerGroomed";
            this.dateTimePickerGroomed.Size = new System.Drawing.Size(127, 20);
            this.dateTimePickerGroomed.TabIndex = 6;
            // 
            // dateTimePickerPregnant
            // 
            this.dateTimePickerPregnant.CustomFormat = "yyyy-MM-dd   HH:mm";
            this.dateTimePickerPregnant.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerPregnant.Location = new System.Drawing.Point(442, 266);
            this.dateTimePickerPregnant.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerPregnant.Name = "dateTimePickerPregnant";
            this.dateTimePickerPregnant.Size = new System.Drawing.Size(127, 20);
            this.dateTimePickerPregnant.TabIndex = 7;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(443, 475);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(68, 32);
            this.buttonOK.TabIndex = 14;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(514, 475);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(68, 32);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxDiseased
            // 
            this.checkBoxDiseased.AutoSize = true;
            this.checkBoxDiseased.Location = new System.Drawing.Point(442, 342);
            this.checkBoxDiseased.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDiseased.Name = "checkBoxDiseased";
            this.checkBoxDiseased.Size = new System.Drawing.Size(68, 17);
            this.checkBoxDiseased.TabIndex = 10;
            this.checkBoxDiseased.Text = "diseased";
            this.checkBoxDiseased.UseVisualStyleBackColor = true;
            // 
            // checkBoxDead
            // 
            this.checkBoxDead.AutoSize = true;
            this.checkBoxDead.Location = new System.Drawing.Point(442, 364);
            this.checkBoxDead.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDead.Name = "checkBoxDead";
            this.checkBoxDead.Size = new System.Drawing.Size(50, 17);
            this.checkBoxDead.TabIndex = 11;
            this.checkBoxDead.Text = "dead";
            this.checkBoxDead.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dateTimePickerBirthDate);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.buttonApplyTraitsFromPasteText);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.textBoxPasteUpdate);
            this.panel1.Controls.Add(this.checkBoxSold);
            this.panel1.Controls.Add(this.textBoxBrandedFor);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.textBoxMate);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.comboBoxAge);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.comboBoxColor);
            this.panel1.Controls.Add(this.checkBoxEpic);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.textBoxComment);
            this.panel1.Controls.Add(this.textBoxCaredForBy);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.numericUpDownAHskill);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.comboBoxFather);
            this.panel1.Controls.Add(this.comboBoxMother);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.checkBoxDead);
            this.panel1.Controls.Add(this.checkedListBoxTraits);
            this.panel1.Controls.Add(this.checkBoxDiseased);
            this.panel1.Controls.Add(this.groupBoxGender);
            this.panel1.Controls.Add(this.textBoxName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dateTimePickerPregnant);
            this.panel1.Controls.Add(this.dateTimePickerGroomed);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.dateTimePickerBred);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Location = new System.Drawing.Point(9, 10);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 460);
            this.panel1.TabIndex = 24;
            // 
            // dateTimePickerBirthDate
            // 
            this.dateTimePickerBirthDate.CustomFormat = "yyyy-MM-dd   HH:mm";
            this.dateTimePickerBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerBirthDate.Location = new System.Drawing.Point(442, 305);
            this.dateTimePickerBirthDate.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerBirthDate.Name = "dateTimePickerBirthDate";
            this.dateTimePickerBirthDate.Size = new System.Drawing.Size(127, 20);
            this.dateTimePickerBirthDate.TabIndex = 47;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(440, 288);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 48;
            this.label16.Text = "Birth date";
            // 
            // buttonApplyTraitsFromPasteText
            // 
            this.buttonApplyTraitsFromPasteText.Location = new System.Drawing.Point(504, 427);
            this.buttonApplyTraitsFromPasteText.Margin = new System.Windows.Forms.Padding(2);
            this.buttonApplyTraitsFromPasteText.Name = "buttonApplyTraitsFromPasteText";
            this.buttonApplyTraitsFromPasteText.Size = new System.Drawing.Size(64, 20);
            this.buttonApplyTraitsFromPasteText.TabIndex = 46;
            this.buttonApplyTraitsFromPasteText.Text = "apply";
            this.buttonApplyTraitsFromPasteText.UseVisualStyleBackColor = true;
            this.buttonApplyTraitsFromPasteText.Click += new System.EventHandler(this.buttonApplyTraitsFromPasteText_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(2, 410);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(270, 13);
            this.label15.TabIndex = 45;
            this.label15.Text = "Update traits by pasting a text line containing traits here:";
            // 
            // textBoxPasteUpdate
            // 
            this.textBoxPasteUpdate.Location = new System.Drawing.Point(4, 427);
            this.textBoxPasteUpdate.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPasteUpdate.Name = "textBoxPasteUpdate";
            this.textBoxPasteUpdate.Size = new System.Drawing.Size(496, 20);
            this.textBoxPasteUpdate.TabIndex = 44;
            this.textBoxPasteUpdate.TabStop = false;
            this.textBoxPasteUpdate.TextChanged += new System.EventHandler(this.textBoxPasteUpdate_TextChanged);
            // 
            // checkBoxSold
            // 
            this.checkBoxSold.AutoSize = true;
            this.checkBoxSold.Location = new System.Drawing.Point(442, 386);
            this.checkBoxSold.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSold.Name = "checkBoxSold";
            this.checkBoxSold.Size = new System.Drawing.Size(45, 17);
            this.checkBoxSold.TabIndex = 43;
            this.checkBoxSold.Text = "sold";
            this.checkBoxSold.UseVisualStyleBackColor = true;
            // 
            // textBoxBrandedFor
            // 
            this.textBoxBrandedFor.Location = new System.Drawing.Point(232, 167);
            this.textBoxBrandedFor.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBrandedFor.Name = "textBoxBrandedFor";
            this.textBoxBrandedFor.Size = new System.Drawing.Size(198, 20);
            this.textBoxBrandedFor.TabIndex = 41;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(230, 151);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 42;
            this.label14.Text = "Branded for";
            // 
            // textBoxMate
            // 
            this.textBoxMate.Location = new System.Drawing.Point(230, 240);
            this.textBoxMate.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMate.Name = "textBoxMate";
            this.textBoxMate.ReadOnly = true;
            this.textBoxMate.Size = new System.Drawing.Size(200, 20);
            this.textBoxMate.TabIndex = 40;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(230, 224);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(159, 13);
            this.label13.TabIndex = 39;
            this.label13.Text = "Paired with (change on list view)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(440, 115);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 13);
            this.label12.TabIndex = 37;
            this.label12.Text = "Age";
            // 
            // comboBoxAge
            // 
            this.comboBoxAge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAge.FormattingEnabled = true;
            this.comboBoxAge.Location = new System.Drawing.Point(442, 131);
            this.comboBoxAge.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxAge.Name = "comboBoxAge";
            this.comboBoxAge.Size = new System.Drawing.Size(127, 21);
            this.comboBoxAge.TabIndex = 36;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(440, 76);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 35;
            this.label11.Text = "Color";
            // 
            // comboBoxColor
            // 
            this.comboBoxColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColor.FormattingEnabled = true;
            this.comboBoxColor.Location = new System.Drawing.Point(442, 93);
            this.comboBoxColor.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxColor.Name = "comboBoxColor";
            this.comboBoxColor.Size = new System.Drawing.Size(127, 21);
            this.comboBoxColor.TabIndex = 34;
            // 
            // checkBoxEpic
            // 
            this.checkBoxEpic.AutoSize = true;
            this.checkBoxEpic.Location = new System.Drawing.Point(292, 205);
            this.checkBoxEpic.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxEpic.Name = "checkBoxEpic";
            this.checkBoxEpic.Size = new System.Drawing.Size(96, 17);
            this.checkBoxEpic.TabIndex = 33;
            this.checkBoxEpic.Text = "epic skill curve";
            this.checkBoxEpic.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(230, 261);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 13);
            this.label10.TabIndex = 32;
            this.label10.Text = "Comments:";
            // 
            // textBoxComment
            // 
            this.textBoxComment.Location = new System.Drawing.Point(230, 277);
            this.textBoxComment.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(198, 132);
            this.textBoxComment.TabIndex = 12;
            // 
            // textBoxCaredForBy
            // 
            this.textBoxCaredForBy.Location = new System.Drawing.Point(232, 131);
            this.textBoxCaredForBy.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCaredForBy.Name = "textBoxCaredForBy";
            this.textBoxCaredForBy.Size = new System.Drawing.Size(198, 20);
            this.textBoxCaredForBy.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(230, 115);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Cared for by";
            // 
            // numericUpDownAHskill
            // 
            this.numericUpDownAHskill.DecimalPlaces = 2;
            this.numericUpDownAHskill.Location = new System.Drawing.Point(230, 204);
            this.numericUpDownAHskill.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownAHskill.Name = "numericUpDownAHskill";
            this.numericUpDownAHskill.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownAHskill.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(230, 188);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Inspected at Animal Husbandry skill";
            // 
            // comboBoxFather
            // 
            this.comboBoxFather.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.comboBoxFather.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxFather.FormattingEnabled = true;
            this.comboBoxFather.Location = new System.Drawing.Point(232, 93);
            this.comboBoxFather.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxFather.Name = "comboBoxFather";
            this.comboBoxFather.Size = new System.Drawing.Size(198, 21);
            this.comboBoxFather.TabIndex = 4;
            this.comboBoxFather.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxFather_Validating);
            // 
            // comboBoxMother
            // 
            this.comboBoxMother.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.comboBoxMother.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxMother.FormattingEnabled = true;
            this.comboBoxMother.Location = new System.Drawing.Point(232, 54);
            this.comboBoxMother.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxMother.Name = "comboBoxMother";
            this.comboBoxMother.Size = new System.Drawing.Size(198, 21);
            this.comboBoxMother.TabIndex = 3;
            this.comboBoxMother.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxMother_Validating);
            // 
            // labelWarn
            // 
            this.labelWarn.AutoSize = true;
            this.labelWarn.ForeColor = System.Drawing.Color.Red;
            this.labelWarn.Location = new System.Drawing.Point(11, 483);
            this.labelWarn.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelWarn.Name = "labelWarn";
            this.labelWarn.Size = new System.Drawing.Size(58, 13);
            this.labelWarn.TabIndex = 26;
            this.labelWarn.Text = "Warn label";
            this.labelWarn.Visible = false;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.Location = new System.Drawing.Point(371, 475);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(68, 32);
            this.buttonEdit.TabIndex = 16;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Visible = false;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // FormHorseViewEdit
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(593, 518);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelWarn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormHorseViewEdit";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Creature";
            this.Load += new System.EventHandler(this.FormHorseViewEdit_Load);
            this.groupBoxGender.ResumeLayout(false);
            this.groupBoxGender.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAHskill)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxTraits;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxGender;
        private System.Windows.Forms.RadioButton radioButtonFemale;
        private System.Windows.Forms.RadioButton radioButtonMale;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dateTimePickerBred;
        private System.Windows.Forms.DateTimePicker dateTimePickerGroomed;
        private System.Windows.Forms.DateTimePicker dateTimePickerPregnant;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxDiseased;
        private System.Windows.Forms.CheckBox checkBoxDead;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.ComboBox comboBoxFather;
        private System.Windows.Forms.ComboBox comboBoxMother;
        private System.Windows.Forms.Label labelWarn;
        private System.Windows.Forms.NumericUpDown numericUpDownAHskill;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCaredForBy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.CheckBox checkBoxEpic;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxColor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxAge;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxMate;
        private System.Windows.Forms.TextBox textBoxBrandedFor;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBoxSold;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxPasteUpdate;
        private System.Windows.Forms.Button buttonApplyTraitsFromPasteText;
        private System.Windows.Forms.DateTimePicker dateTimePickerBirthDate;
        private System.Windows.Forms.Label label16;
    }
}