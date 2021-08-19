namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastNameToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastName");
        }
    }
}
