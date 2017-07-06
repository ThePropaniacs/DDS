namespace DDSDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("TimeSheet", "Email", x =>x.String());
        }

        public override void Down()
        {
            DropColumn("TimeSheet", "Email");
        }
    }
}
