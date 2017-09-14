namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartingOverAgain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        CompanyName = c.String(),
                        EmployerName = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        StartTime = c.DateTime(),
                        StopTime = c.DateTime(),
                        Note = c.String(),
                        ClientFeedback = c.String(),
                        Approved = c.Boolean(),
                        ApprovedBy = c.String(),
                        ApprovedDate = c.DateTime(),
                        Processed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Sun = c.Boolean(),
                        Mon = c.Boolean(),
                        Tue = c.Boolean(),
                        Wed = c.Boolean(),
                        Thu = c.Boolean(),
                        Fri = c.Boolean(),
                        Sat = c.Boolean(),
                        AvailNotes = c.String(),
                        AvailStart = c.DateTime(),
                        AvailExpires = c.DateTime(),
                        AvailDuration = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Placements",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        CompanyName = c.String(),
                        ClientId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        PlacementDate = c.DateTime(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Placements", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Placements", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.TimeSheets", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.TimeSheets", "ClientId", "dbo.Clients");
            DropIndex("dbo.Placements", new[] { "EmployeeId" });
            DropIndex("dbo.Placements", new[] { "ClientId" });
            DropIndex("dbo.TimeSheets", new[] { "ClientId" });
            DropIndex("dbo.TimeSheets", new[] { "EmployeeId" });
            DropTable("dbo.Placements");
            DropTable("dbo.Employees");
            DropTable("dbo.TimeSheets");
            DropTable("dbo.Clients");
        }
    }
}
