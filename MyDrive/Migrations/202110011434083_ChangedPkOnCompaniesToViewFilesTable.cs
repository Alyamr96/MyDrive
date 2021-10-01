namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPkOnCompaniesToViewFilesTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CompaniesToViewFiles");
            AlterColumn("dbo.CompaniesToViewFiles", "CompanyName", c => c.String());
            AlterColumn("dbo.CompaniesToViewFiles", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CompaniesToViewFiles", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CompaniesToViewFiles");
            AlterColumn("dbo.CompaniesToViewFiles", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.CompaniesToViewFiles", "CompanyName", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.CompaniesToViewFiles", "CompanyName");
        }
    }
}
