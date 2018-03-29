namespace Project1Afdemp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createUserAndMessage : DbMigration
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
                        Receiver_UserName = c.String(maxLength: 128),
                        Sender_Id = c.Int(),
                        Sender_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => new { t.Receiver_Id, t.Receiver_UserName })
                .ForeignKey("dbo.Users", t => new { t.Sender_Id, t.Sender_UserName })
                .Index(t => new { t.Receiver_Id, t.Receiver_UserName })
                .Index(t => new { t.Sender_Id, t.Sender_UserName });
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Password = c.String(),
                        UserAccess = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.UserName });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", new[] { "Sender_Id", "Sender_UserName" }, "dbo.Users");
            DropForeignKey("dbo.Messages", new[] { "Receiver_Id", "Receiver_UserName" }, "dbo.Users");
            DropIndex("dbo.Messages", new[] { "Sender_Id", "Sender_UserName" });
            DropIndex("dbo.Messages", new[] { "Receiver_Id", "Receiver_UserName" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
