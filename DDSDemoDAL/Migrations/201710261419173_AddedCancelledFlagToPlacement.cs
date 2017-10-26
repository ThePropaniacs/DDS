namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCancelledFlagToPlacement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Placements", "Cancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Placements", "Cancelled");
        }
    }
}
