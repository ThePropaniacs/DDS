namespace DDSDemoDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblClientMaster",
                c => new
                    {
                        ID = c.Decimal(nullable: false, precision: 18, scale: 0, identity: true, storeType: "numeric"),
                        CompanyName = c.String(nullable: false, maxLength: 50, unicode: false),
                        EmployerID = c.Int(nullable: false),
                        EmployerName = c.String(maxLength: 50, unicode: false),
                        Address1 = c.String(maxLength: 50, unicode: false),
                        Address2 = c.String(maxLength: 50, unicode: false),
                        City = c.String(maxLength: 50, unicode: false),
                        State = c.String(maxLength: 2, fixedLength: true, unicode: false),
                        Zip = c.String(maxLength: 10, unicode: false),
                        Email = c.String(maxLength: 50, unicode: false),
                        Phone = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.tblTimeSheetMaster",
                c => new
                    {
                        ID = c.Decimal(nullable: false, precision: 18, scale: 0, identity: true, storeType: "numeric"),
                        CompanyName = c.String(maxLength: 50, unicode: false),
                        EmpID = c.Decimal(nullable: false, precision: 18, scale: 0, storeType: "numeric"),
                        AssocClientID = c.Decimal(precision: 18, scale: 0, storeType: "numeric"),
                        StartTime = c.DateTime(),
                        StopTime = c.DateTime(),
                        Note = c.String(maxLength: 150, unicode: false),
                        Approved = c.Boolean(),
                        ApprovedBy = c.Int(nullable: false),
                        ApprovedDate = c.DateTime(),
                        Processed = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tblEmployeeMaster", t => t.EmpID)
                .ForeignKey("dbo.tblClientMaster", t => t.AssocClientID)
                .Index(t => t.EmpID)
                .Index(t => t.AssocClientID);
            
            CreateTable(
                "dbo.tblEmployeeMaster",
                c => new
                    {
                        ID = c.Decimal(nullable: false, precision: 18, scale: 0, identity: true, storeType: "numeric"),
                        CompanyName = c.String(maxLength: 50, unicode: false),
                        EmpID = c.Int(nullable: false),
                        FirstName = c.String(maxLength: 50, unicode: false),
                        LastName = c.String(maxLength: 50, unicode: false),
                        Sun = c.Boolean(),
                        Mon = c.Boolean(),
                        Tue = c.Boolean(),
                        Wed = c.Boolean(),
                        Thu = c.Boolean(),
                        Fri = c.Boolean(),
                        Sat = c.Boolean(),
                        AvailNotes = c.String(maxLength: 50, unicode: false),
                        AvailStart = c.DateTime(),
                        AvailExpires = c.DateTime(),
                        AvailDuration = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Placements",
                c => new
                    {
                        ID = c.Decimal(nullable: false, precision: 18, scale: 0, identity: true, storeType: "numeric"),
                        CompanyName = c.String(maxLength: 50, unicode: false),
                        PlacementID = c.Int(),
                        ClientID = c.Int(),
                        EmployeeID = c.Int(),
                        PlacementDate = c.DateTime(storeType: "date"),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        Position = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblTimeSheetMaster", "AssocClientID", "dbo.tblClientMaster");
            DropForeignKey("dbo.tblTimeSheetMaster", "EmpID", "dbo.tblEmployeeMaster");
            DropIndex("dbo.tblTimeSheetMaster", new[] { "AssocClientID" });
            DropIndex("dbo.tblTimeSheetMaster", new[] { "EmpID" });
            DropTable("dbo.Placements");
            DropTable("dbo.tblEmployeeMaster");
            DropTable("dbo.tblTimeSheetMaster");
            DropTable("dbo.tblClientMaster");
        }
    }
}
