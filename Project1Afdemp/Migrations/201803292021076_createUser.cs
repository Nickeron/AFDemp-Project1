namespace Project1Afdemp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeSent = c.DateTime(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        Receiver_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Receiver_Id)
                .ForeignKey("dbo.Users", t => t.Sender_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 20),
                        Password = c.String(),
                        UserAccess = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Sender_Id", "dbo.Users");
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.Users");
            DropIndex("dbo.Users", new[] { "UserName" });
            DropIndex("dbo.Messages", new[] { "Sender_Id" });
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
