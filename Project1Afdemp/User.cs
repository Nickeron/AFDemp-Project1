using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1Afdemp
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; set; }

        private User() { }

        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }

        public User(string username, string password, Accessibility userAccess)
        {
            UserName = username;
            Password = password;
            UserAccess = userAccess;
        }
    }
}
