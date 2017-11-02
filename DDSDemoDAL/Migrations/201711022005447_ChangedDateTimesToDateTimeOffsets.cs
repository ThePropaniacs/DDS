namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedDateTimesToDateTimeOffsets : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TimeSheets", "StartTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.TimeSheets", "StopTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.TimeSheets", "ApprovedDate", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TimeSheets", "ApprovedDate", c => c.DateTime());
            AlterColumn("dbo.TimeSheets", "StopTime", c => c.DateTime());
            AlterColumn("dbo.TimeSheets", "StartTime", c => c.DateTime());
        }
    }
}
