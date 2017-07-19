namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppingEmail : DbMigration
    {
        public override void Up()
        {
            DropColumn("tblTimeSheetMaster", "Email");
        }
        
        public override void Down()
        {
        }
    }
}
