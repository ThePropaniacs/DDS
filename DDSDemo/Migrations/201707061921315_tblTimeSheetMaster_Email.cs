namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblTimeSheetMaster_Email : DbMigration
    {
        public override void Up()
        {
            AddColumn("tblTimeSheetMaster", "Email", x => x.String());
        }
        
        public override void Down()
        {
            DropColumn("tblTimeSheetMaster", "Email");
        }
    }
}
