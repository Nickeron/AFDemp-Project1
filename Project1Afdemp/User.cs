using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;

namespace Project1Afdemp
{
    public class User
    {
        public int Id { get; set; }
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
