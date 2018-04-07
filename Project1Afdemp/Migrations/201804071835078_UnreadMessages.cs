namespace Project1Afdemp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnreadMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IdsUnreadChatMessages", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IdsUnreadChatMessages");
        }
    }
}
