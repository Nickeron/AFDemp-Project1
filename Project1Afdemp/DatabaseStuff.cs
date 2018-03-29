using System.Data.Entity;

namespace Project1Afdemp
{
    public class DatabaseStuff : DbContext
    {
        public DatabaseStuff() : base("Users")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseStuff, EF6Console.Migrations.Configuration>());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
