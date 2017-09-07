namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedClientFeedbackToTimeSheet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblTimeSheetMaster", "ClientFeedback", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblTimeSheetMaster", "ClientFeedback");
        }
    }
}
