using System;
using System.Linq;
using System.Security;

namespace Project1Afdemp
{
    class UserManager
    {
        public User TheUser { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; private set; }
        private static DatabaseStuff UserDatabase { get; set; }

        static UserManager()
        {
            UserDatabase = new DatabaseStuff();
        }

        public UserManager(string userName, SecureString password, bool isNewUser = false)
        {
            if (IsWrongUserName(userName, isNewUser))
            {
                AskUserName(isNewUser);
            }
            else
            {
                UserName = userName;
            }
            if (UserName != "guest" || IsWrongPassword(password, isNewUser))
            {
                AskPassword(isNewUser);
            }
            else
            {
                Password = PasswordHandling.SecureStringToString(password, UserName);
            }
            SetAccessibility();
            TheUser = new User(UserName, Password, UserAccess);
        }

        public UserManager(bool isNewUser = false) : this("", new SecureString(), isNewUser) { }

        private void AskUserName(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tUser Name:\t");
            string userName = Console.ReadLine();
            while (IsWrongUserName(userName, isNewUser))
            {
                Console.Write("\n\n\tUser Name:\t");
                userName = Console.ReadLine();
            }
            UserName = userName;
        }

        private bool IsWrongUserName(string userName, bool isNewUser)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            if (userName.Length < 5 || userName.Length > 20)
            {
                Console.Write("\n\n\tUser Name has to be between 5 and 20 characters long!");
                Console.ResetColor();
                return true;
            }
            else if (UserNameAlreadyExists(userName) && isNewUser)
            {
                Console.Write("\n\n\tUser Name already exists! Try another");
                Console.ResetColor();
                return true;
            }
            else if (!UserNameAlreadyExists(userName) && !isNewUser)
            {
                Console.Write("\n\n\tThat username doesn't exist! Check again.");
                Console.ResetColor();
                return true;
            }
            Console.ResetColor();
            return false;
        }

        private bool UserNameAlreadyExists(string userName)
        {
            return UserDatabase.Users.Any(i => i.UserName == userName);
        }

        private void AskPassword(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tPassword:\t");
            var password = PasswordHandling.GetPassword();
            while (IsWrongPassword(password, isNewUser))
            {
                Console.Write("\n\n\tPassword:\t");
                password = PasswordHandling.GetPassword();
            }
            Password = PasswordHandling.SecureStringToString(password, UserName);
        }

        private bool IsWrongPassword(SecureString password, bool isNewUser)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            if (password.Length < 5 || password.Length > 20)
            {
                Console.Write("\n\n\tPassword has to be between 5 and 20 characters long!");
                Console.ResetColor();
                return true;
            }
            else if (!isNewUser && !IDmatched(password))
            {
                Console.Write("\n\n\tWrong Password! Try again.");
                Console.ResetColor();
                return true;
            }
            Console.ResetColor();
            return false;
        }

        private bool IDmatched(SecureString password)
        {
            string passHash = UserDatabase.Users.Single(i => i.UserName == UserName).Password;
            return (PasswordHandling.SecureStringToString(password, UserName) == passHash);
        }

        private void SetAccessibility()
        {
            if (UserName == "admin")
            {
                UserAccess = Accessibility.administrator;
            }
            else if (UserName == "guest")
            {
                UserAccess = Accessibility.guest;
            }
            else
            {
                UserAccess = Accessibility.user;
            }
        }

    }
}
