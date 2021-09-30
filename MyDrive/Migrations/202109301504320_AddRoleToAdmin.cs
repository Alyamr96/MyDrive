namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleToAdmin : DbMigration
    {
        public override void Up()
        {
            Sql(@"
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'9c8626af-675d-4114-857a-97e6c1a11156', N'CanDoEverything')
SET IDENTITY_INSERT [dbo].[RolePermissions] ON
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (13, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Assign Users To Companies')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (14, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Create Folder')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (15, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Create Role')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (16, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Create User')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (17, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete Company')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (18, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete File')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (19, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete Folder')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (20, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete Multiple FoldersAndFiles')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (21, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete Role')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (22, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Delete User')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (23, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Edit Role')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (24, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Manage Companies')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (25, N'9c8626af-675d-4114-857a-97e6c1a11156', N'Upload File')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (26, N'9c8626af-675d-4114-857a-97e6c1a11156', N'View All Uploaded Files')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (27, N'9c8626af-675d-4114-857a-97e6c1a11156', N'View Filters')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (28, N'9c8626af-675d-4114-857a-97e6c1a11156', N'View Roles')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (29, N'9c8626af-675d-4114-857a-97e6c1a11156', N'View Users')
INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionName]) VALUES (30, N'9c8626af-675d-4114-857a-97e6c1a11156', N'View Users')
SET IDENTITY_INSERT [dbo].[RolePermissions] OFF
");
        }
        
        public override void Down()
        {
        }
    }
}
