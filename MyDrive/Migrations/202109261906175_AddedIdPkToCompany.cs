namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIdPkToCompany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Company_Path", "dbo.Companies");
            DropIndex("dbo.AspNetUsers", new[] { "Company_Path" });
            RenameColumn(table: "dbo.AspNetUsers", name: "Company_Path", newName: "Company_Id");
            DropPrimaryKey("dbo.Companies");
            AddColumn("dbo.Companies", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Companies", "Path", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Company_Id", c => c.Int());
            AddPrimaryKey("dbo.Companies", "Id");
            CreateIndex("dbo.AspNetUsers", "Company_Id");
            AddForeignKey("dbo.AspNetUsers", "Company_Id", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Company_Id", "dbo.Companies");
            DropIndex("dbo.AspNetUsers", new[] { "Company_Id" });
            DropPrimaryKey("dbo.Companies");
            AlterColumn("dbo.AspNetUsers", "Company_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Companies", "Path", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Companies", "Id");
            AddPrimaryKey("dbo.Companies", "Path");
            RenameColumn(table: "dbo.AspNetUsers", name: "Company_Id", newName: "Company_Path");
            CreateIndex("dbo.AspNetUsers", "Company_Path");
            AddForeignKey("dbo.AspNetUsers", "Company_Path", "dbo.Companies", "Path");
        }
    }
}
