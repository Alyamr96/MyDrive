namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserIdToFileModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileModels", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileModels", "UserId");
        }
    }
}
