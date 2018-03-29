using System.Data.Entity;

namespace Project1Afdemp
{
    public class DatabaseStuff : DbContext
    {
        public DatabaseStuff() : base("Users")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
