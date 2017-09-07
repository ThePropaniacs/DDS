namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetProcessedToNonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblTimeSheetMaster", "Processed", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblTimeSheetMaster", "Processed", c => c.Boolean());
        }
    }
}
