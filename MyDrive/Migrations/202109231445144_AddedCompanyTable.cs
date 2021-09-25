namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompanyTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Path = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        LogoPath = c.String(),
                    })
                .PrimaryKey(t => t.Path);
            
            AddColumn("dbo.AspNetUsers", "Company_Path", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "Company_Path");
            AddForeignKey("dbo.AspNetUsers", "Company_Path", "dbo.Companies", "Path");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Company_Path", "dbo.Companies");
            DropIndex("dbo.AspNetUsers", new[] { "Company_Path" });
            DropColumn("dbo.AspNetUsers", "Company_Path");
            DropTable("dbo.Companies");
        }
    }
}
