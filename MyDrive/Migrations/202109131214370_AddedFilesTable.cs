namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFilesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileModels",
                c => new
                    {
                        Path = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Path);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileModels");
        }
    }
}
