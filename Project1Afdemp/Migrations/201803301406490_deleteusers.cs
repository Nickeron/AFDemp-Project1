namespace Project1Afdemp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteusers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "ReceiverId", "dbo.Users");
            DropForeignKey("dbo.Messages", "SenderId", "dbo.Users");
            AddForeignKey("dbo.Messages", "ReceiverId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Messages", "SenderId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderId", "dbo.Users");
            DropForeignKey("dbo.Messages", "ReceiverId", "dbo.Users");
            AddForeignKey("dbo.Messages", "SenderId", "dbo.Users", "Id");
            AddForeignKey("dbo.Messages", "ReceiverId", "dbo.Users", "Id");
        }
    }
}
