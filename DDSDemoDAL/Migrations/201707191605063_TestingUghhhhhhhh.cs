namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestingUghhhhhhhh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblTimeSheetMaster", "ApprovedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblTimeSheetMaster", "ApprovedBy");
        }
    }
}
