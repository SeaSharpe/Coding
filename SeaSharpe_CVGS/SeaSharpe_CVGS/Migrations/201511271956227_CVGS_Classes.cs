namespace SeaSharpe_CVGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CVGS_Classes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StreetAddress = c.String(nullable: false, maxLength: 255),
                        City = c.String(nullable: false, maxLength: 50),
                        Region = c.String(nullable: false, maxLength: 50),
                        Country = c.String(nullable: false, maxLength: 50),
                        PostalCode = c.String(nullable: false, maxLength: 6, fixedLength: true, unicode: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsEmailVerified = c.Boolean(nullable: false),
                        IsEmailMarketingAllowed = c.Boolean(nullable: false),
                        StripeID = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(maxLength: 2000),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 4000),
                        Capacity = c.Int(nullable: false),
                        Employee_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id, cascadeDelete: true)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Friendships",
                c => new
                    {
                        Friendee_Id = c.Int(nullable: false),
                        Friender_Id = c.Int(nullable: false),
                        IsFamilyMember = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.Friendee_Id, t.Friender_Id })
                .ForeignKey("dbo.Members", t => t.Friender_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Friendee_Id)
                .Index(t => t.Friendee_Id)
                .Index(t => t.Friender_Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ImagePath = c.String(),
                        Publisher = c.String(maxLength: 50),
                        ESRB = c.String(maxLength: 4),
                        ReleaseDate = c.DateTime(nullable: false),
                        SuggestedRetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Platform_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Platforms", t => t.Platform_Id, cascadeDelete: true)
                .Index(t => t.Platform_Id);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rating = c.Single(nullable: false),
                        Subject = c.String(maxLength: 500),
                        Body = c.String(maxLength: 4000),
                        IsApproved = c.Boolean(nullable: false),
                        Aprover_Id = c.Int(),
                        Author_Id = c.Int(nullable: false),
                        Game_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Aprover_Id)
                .ForeignKey("dbo.Members", t => t.Author_Id, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .Index(t => t.Aprover_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Game_Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Game_Id = c.Int(nullable: false),
                        Order_Id = c.Int(nullable: false),
                        SalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.Game_Id, t.Order_Id })
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.Order_Id, cascadeDelete: true)
                .Index(t => t.Game_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderPlacementDate = c.DateTime(),
                        ShipDate = c.DateTime(),
                        IsProcessed = c.Boolean(nullable: false),
                        Aprover_Id = c.Int(),
                        BillingAddress_Id = c.Int(),
                        Member_Id = c.Int(),
                        ShippingAddress_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Aprover_Id)
                .ForeignKey("dbo.Addresses", t => t.BillingAddress_Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddress_Id)
                .Index(t => t.Aprover_Id)
                .Index(t => t.BillingAddress_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.ShippingAddress_Id);
            
            CreateTable(
                "dbo.WishLists",
                c => new
                    {
                        Member_Id = c.Int(nullable: false),
                        Game_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Member_Id, t.Game_Id })
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.Member_Id)
                .Index(t => t.Game_Id);
            
            CreateTable(
                "dbo.EventMembers",
                c => new
                    {
                        Event_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_Id, t.Member_Id })
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .Index(t => t.Event_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.GameCategories",
                c => new
                    {
                        Game_Id = c.Int(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Game_Id, t.Category_Id })
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Category_Id);

            Sql("DBCC CHECKIDENT ( 'Employees'  , RESEED, 200001 )");
            Sql("DBCC CHECKIDENT ( 'Members'    , RESEED, 30000001 )");
            Sql("DBCC CHECKIDENT ( 'Events'     , RESEED, 4000001 )");
            Sql("DBCC CHECKIDENT ( 'Addresses'  , RESEED, 500000001 )");
            Sql("DBCC CHECKIDENT ( 'Orders'     , RESEED, 600000001 )");
            Sql("DBCC CHECKIDENT ( 'Games'      , RESEED, 7000001 )");
            Sql("DBCC CHECKIDENT ( 'Platforms'  , RESEED, 801 )");
            Sql("DBCC CHECKIDENT ( 'Categories' , RESEED, 8001 )");
            Sql("DBCC CHECKIDENT ( 'Reviews'    , RESEED, 900000001 )");
            
            AddColumn("dbo.AspNetUsers", "Gender", c => c.String(nullable: false, maxLength: 1, fixedLength: true, unicode: false));
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateOfRegistration", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.WishLists", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.WishLists", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Orders", "ShippingAddress_Id", "dbo.Addresses");
            DropForeignKey("dbo.OrderItems", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.Orders", "BillingAddress_Id", "dbo.Addresses");
            DropForeignKey("dbo.Orders", "Aprover_Id", "dbo.Employees");
            DropForeignKey("dbo.OrderItems", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Reviews", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Reviews", "Author_Id", "dbo.Members");
            DropForeignKey("dbo.Reviews", "Aprover_Id", "dbo.Employees");
            DropForeignKey("dbo.Games", "Platform_Id", "dbo.Platforms");
            DropForeignKey("dbo.GameCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.GameCategories", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Addresses", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.Members", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Friendships", "Friendee_Id", "dbo.Members");
            DropForeignKey("dbo.Friendships", "Friender_Id", "dbo.Members");
            DropForeignKey("dbo.Events", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.Employees", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.EventMembers", "Event_Id", "dbo.Events");
            DropIndex("dbo.GameCategories", new[] { "Category_Id" });
            DropIndex("dbo.GameCategories", new[] { "Game_Id" });
            DropIndex("dbo.EventMembers", new[] { "Member_Id" });
            DropIndex("dbo.EventMembers", new[] { "Event_Id" });
            DropIndex("dbo.WishLists", new[] { "Game_Id" });
            DropIndex("dbo.WishLists", new[] { "Member_Id" });
            DropIndex("dbo.Orders", new[] { "ShippingAddress_Id" });
            DropIndex("dbo.Orders", new[] { "Member_Id" });
            DropIndex("dbo.Orders", new[] { "BillingAddress_Id" });
            DropIndex("dbo.Orders", new[] { "Aprover_Id" });
            DropIndex("dbo.OrderItems", new[] { "Order_Id" });
            DropIndex("dbo.OrderItems", new[] { "Game_Id" });
            DropIndex("dbo.Reviews", new[] { "Game_Id" });
            DropIndex("dbo.Reviews", new[] { "Author_Id" });
            DropIndex("dbo.Reviews", new[] { "Aprover_Id" });
            DropIndex("dbo.Games", new[] { "Platform_Id" });
            DropIndex("dbo.Friendships", new[] { "Friender_Id" });
            DropIndex("dbo.Friendships", new[] { "Friendee_Id" });
            DropIndex("dbo.Employees", new[] { "User_Id" });
            DropIndex("dbo.Events", new[] { "Employee_Id" });
            DropIndex("dbo.Members", new[] { "User_Id" });
            DropIndex("dbo.Addresses", new[] { "Member_Id" });
            DropColumn("dbo.AspNetUsers", "DateOfRegistration");
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "Gender");
            DropTable("dbo.GameCategories");
            DropTable("dbo.EventMembers");
            DropTable("dbo.WishLists");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Reviews");
            DropTable("dbo.Platforms");
            DropTable("dbo.Games");
            DropTable("dbo.Categories");
            DropTable("dbo.Friendships");
            DropTable("dbo.Employees");
            DropTable("dbo.Events");
            DropTable("dbo.Members");
            DropTable("dbo.Addresses");
        }
    }
}
