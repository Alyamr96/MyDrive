namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeleteFoldersRole : DbMigration
    {
        public override void Up()
        {
            Sql(@"
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'95eabe84-6928-4be4-9eeb-d6457bebe6e7', N'DeleteFolders')
");
        }
        
        public override void Down()
        {
        }
    }
}
