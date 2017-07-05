namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoolMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("[dbo].[tblEmployeeMaster]", "Sun", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("[dbo].[tblEmployeeMaster]", "Sun", c => c.Int(nullable: false));
        }
    }
}
