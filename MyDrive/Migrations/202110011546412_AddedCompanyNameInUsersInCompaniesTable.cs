namespace MyDrive.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompanyNameInUsersInCompaniesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UsersInCompanies", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UsersInCompanies", "CompanyName");
        }
    }
}
