namespace AldursLab.WurmAssistantWebService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyWurmAssistant3PackagesAndAddLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Source = c.String(maxLength: 2048),
                        Content = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.LogId);
            
            AddColumn("dbo.WurmAssistantPackages", "BuildCode", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.WurmAssistantPackages", "BuildNumber", c => c.String(nullable: false, maxLength: 250));
            DropColumn("dbo.WurmAssistantPackages", "VersionString");
            DropColumn("dbo.WurmAssistantPackages", "ProjectType");
            DropColumn("dbo.WurmAssistantPackages", "ReleaseType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WurmAssistantPackages", "ReleaseType", c => c.Int(nullable: false));
            AddColumn("dbo.WurmAssistantPackages", "ProjectType", c => c.Int(nullable: false));
            AddColumn("dbo.WurmAssistantPackages", "VersionString", c => c.String(nullable: false));
            DropColumn("dbo.WurmAssistantPackages", "BuildNumber");
            DropColumn("dbo.WurmAssistantPackages", "BuildCode");
            DropTable("dbo.Logs");
        }
    }
}
