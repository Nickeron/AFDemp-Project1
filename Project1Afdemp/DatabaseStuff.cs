using System.Data.Entity;

namespace Project1Afdemp
{
    public class DatabaseStuff : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatMessage> Chat { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasKey(m => m.Id)
                        .HasMany(s => s.UnreadChatMessages)
                        .WithMany(c => c.UnreadUsers)
                        .Map(cs =>
                        {
                            cs.MapLeftKey("UserRefId");
                            cs.MapRightKey("ChatMessageRefId");
                            cs.ToTable("UnreadChatMessages");
                        });

            modelBuilder.Entity<Message>()
                        .HasRequired(m => m.Sender)
                        .WithMany(t => t.SentMessages)
                        .HasForeignKey(m => m.SenderId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                        .HasRequired(m => m.Receiver)
                        .WithMany(t => t.ReceivedMessages)
                        .HasForeignKey(m => m.ReceiverId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMessage>()
                        .HasRequired(m => m.Sender)
                        .WithMany(t => t.SentChatMessages)
                        .HasForeignKey(m => m.SenderId)
                        .WillCascadeOnDelete(false);
        }
    }
}
