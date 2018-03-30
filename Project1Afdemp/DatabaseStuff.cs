using System.Data.Entity;

namespace Project1Afdemp
{
    public class DatabaseStuff : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasKey(m => m.Id);

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
        }
    }
}
