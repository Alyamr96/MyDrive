namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompaniesToViewFilesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompaniesToViewFiles",
                c => new
                    {
                        CompanyName = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.CompanyName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompaniesToViewFiles");
        }
    }
}
