using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1Afdemp
{
    public class DatabaseStuff : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
