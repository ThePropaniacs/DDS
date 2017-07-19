namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedApprovedByToApprovedName : DbMigration
    {
        public override void Up()
        {
            DropColumn("tblTimeSheetMaster", "ApprovedBy");
            AddColumn("tblTimeSheetMaster", "ApprovedName", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
