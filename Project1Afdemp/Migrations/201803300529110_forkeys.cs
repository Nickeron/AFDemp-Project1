namespace Project1Afdemp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forkeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            DropIndex("dbo.Messages", new[] { "Sender_Id" });
            DropIndex("dbo.Users", new[] { "UserName" });
            RenameColumn(table: "dbo.Messages", name: "Receiver_Id", newName: "ReceiverId");
            RenameColumn(table: "dbo.Messages", name: "Sender_Id", newName: "SenderId");
            AlterColumn("dbo.Messages", "ReceiverId", c => c.Int(nullable: false));
            AlterColumn("dbo.Messages", "SenderId", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "UserName", c => c.String());
            CreateIndex("dbo.Messages", "SenderId");
            CreateIndex("dbo.Messages", "ReceiverId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Messages", new[] { "ReceiverId" });
            DropIndex("dbo.Messages", new[] { "SenderId" });
            AlterColumn("dbo.Users", "UserName", c => c.String(maxLength: 20));
            AlterColumn("dbo.Messages", "SenderId", c => c.Int());
            AlterColumn("dbo.Messages", "ReceiverId", c => c.Int());
            RenameColumn(table: "dbo.Messages", name: "SenderId", newName: "Sender_Id");
            RenameColumn(table: "dbo.Messages", name: "ReceiverId", newName: "Receiver_Id");
            CreateIndex("dbo.Users", "UserName", unique: true);
            CreateIndex("dbo.Messages", "Sender_Id");
            CreateIndex("dbo.Messages", "Receiver_Id");
        }
    }
}
