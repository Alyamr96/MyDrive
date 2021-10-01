namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompaniesToViewFile2Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompaniesToViewFiles2",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompaniesToViewFiles2");
        }
    }
}
