namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdminToDoEverythingRole : DbMigration
    {
        public override void Up()
        {
            Sql(@"
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'a26e8a5a-1609-459f-a213-2d1ad04c8ab6', N'9c8626af-675d-4114-857a-97e6c1a11156')
");
        }
        
        public override void Down()
        {
        }
    }
}
