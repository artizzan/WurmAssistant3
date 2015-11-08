namespace AldursLab.WurmAssistant3.Core.Areas.MainMenu.Views
{
    partial class MenuView
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyServersListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.officialForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pMAldurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contributorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donatorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roadmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewRoadmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importDataFromWa2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.creditsToolStripMenuItem,
            this.roadmapToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(278, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeSettingsToolStripMenuItem,
            this.modifyServersListToolStripMenuItem,
            this.toolStripSeparator1,
            this.importDataFromWa2ToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // changeSettingsToolStripMenuItem
            // 
            this.changeSettingsToolStripMenuItem.Name = "changeSettingsToolStripMenuItem";
            this.changeSettingsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.changeSettingsToolStripMenuItem.Text = "Change settings";
            this.changeSettingsToolStripMenuItem.Click += new System.EventHandler(this.changeSettingsToolStripMenuItem_Click);
            // 
            // modifyServersListToolStripMenuItem
            // 
            this.modifyServersListToolStripMenuItem.Name = "modifyServersListToolStripMenuItem";
            this.modifyServersListToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.modifyServersListToolStripMenuItem.Text = "Modify servers list";
            this.modifyServersListToolStripMenuItem.Click += new System.EventHandler(this.modifyServersListToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.officialForumToolStripMenuItem,
            this.wikiToolStripMenuItem,
            this.pMAldurToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // officialForumToolStripMenuItem
            // 
            this.officialForumToolStripMenuItem.Name = "officialForumToolStripMenuItem";
            this.officialForumToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.officialForumToolStripMenuItem.Text = "Official Forum";
            this.officialForumToolStripMenuItem.Click += new System.EventHandler(this.officialForumToolStripMenuItem_Click);
            // 
            // wikiToolStripMenuItem
            // 
            this.wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            this.wikiToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.wikiToolStripMenuItem.Text = "Wiki";
            this.wikiToolStripMenuItem.Click += new System.EventHandler(this.wikiToolStripMenuItem_Click);
            // 
            // pMAldurToolStripMenuItem
            // 
            this.pMAldurToolStripMenuItem.Name = "pMAldurToolStripMenuItem";
            this.pMAldurToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.pMAldurToolStripMenuItem.Text = "PM Aldur";
            this.pMAldurToolStripMenuItem.Click += new System.EventHandler(this.pMAldurToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contributorsToolStripMenuItem,
            this.donatorsToolStripMenuItem});
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.creditsToolStripMenuItem.Text = "Credits";
            // 
            // contributorsToolStripMenuItem
            // 
            this.contributorsToolStripMenuItem.Name = "contributorsToolStripMenuItem";
            this.contributorsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.contributorsToolStripMenuItem.Text = "Contributors";
            this.contributorsToolStripMenuItem.Click += new System.EventHandler(this.contributorsToolStripMenuItem_Click);
            // 
            // donatorsToolStripMenuItem
            // 
            this.donatorsToolStripMenuItem.Name = "donatorsToolStripMenuItem";
            this.donatorsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.donatorsToolStripMenuItem.Text = "Donators";
            this.donatorsToolStripMenuItem.Click += new System.EventHandler(this.donatorsToolStripMenuItem_Click);
            // 
            // roadmapToolStripMenuItem
            // 
            this.roadmapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewRoadmapToolStripMenuItem});
            this.roadmapToolStripMenuItem.Name = "roadmapToolStripMenuItem";
            this.roadmapToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.roadmapToolStripMenuItem.Text = "Roadmap";
            // 
            // viewRoadmapToolStripMenuItem
            // 
            this.viewRoadmapToolStripMenuItem.Name = "viewRoadmapToolStripMenuItem";
            this.viewRoadmapToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.viewRoadmapToolStripMenuItem.Text = "View roadmap";
            this.viewRoadmapToolStripMenuItem.Click += new System.EventHandler(this.viewRoadmapToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // importDataFromWa2ToolStripMenuItem
            // 
            this.importDataFromWa2ToolStripMenuItem.Name = "importDataFromWa2ToolStripMenuItem";
            this.importDataFromWa2ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.importDataFromWa2ToolStripMenuItem.Text = "Import data from WA 2";
            this.importDataFromWa2ToolStripMenuItem.Click += new System.EventHandler(this.importDataFromWa2ToolStripMenuItem_Click);
            // 
            // MenuView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip);
            this.Name = "MenuView";
            this.Size = new System.Drawing.Size(278, 33);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem officialForumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wikiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pMAldurToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contributorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donatorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roadmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewRoadmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modifyServersListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem importDataFromWa2ToolStripMenuItem;
    }
}
