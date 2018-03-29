using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1Afdemp
{
    public class User
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Key, Column(Order = 1)]
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; private set; }

        private User() { }

        public User(string username, string password, Accessibility userAccess)
        {
            UserName = username;
            Password = password;
            UserAccess = userAccess;
        }
    }
}
