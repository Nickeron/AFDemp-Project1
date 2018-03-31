using System.Collections.Generic;

namespace Project1Afdemp
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; set; }

        private User() { }

        // Virtual in this context means nullable
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
