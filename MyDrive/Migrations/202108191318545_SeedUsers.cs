namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'a26e8a5a-1609-459f-a213-2d1ad04c8ab6', N'admin@mydrive.com', 0, N'ABBm6pCnOCdemnJVq5EZsQhwTKWTWDfQQ8OXH2FHg6qZDfj+A67Nvh7R92kgBQ7w5w==', N'84d9ca37-0eb8-4c0f-a285-170e9beee172', NULL, 0, 0, NULL, 1, 0, N'admin@mydrive.com')


INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'46aa304f-5a74-4e29-94a3-0a45daa08da4', N'CanManageUsers')

INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'a26e8a5a-1609-459f-a213-2d1ad04c8ab6', N'46aa304f-5a74-4e29-94a3-0a45daa08da4')

");
        }
        
        public override void Down()
        {
        }
    }
}
