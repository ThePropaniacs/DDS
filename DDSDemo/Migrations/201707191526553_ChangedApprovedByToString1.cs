namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedApprovedByToString1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("tblTimeSheetMaster", "ApprovedBy", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
