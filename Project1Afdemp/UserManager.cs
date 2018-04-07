using System;
using System.Linq;

namespace Project1Afdemp
{
    class UserManager
    {
        public User TheUser { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public Accessibility UserAccess { get; private set; }
        private static DatabaseStuff UserDatabase { get; set; }
        private int LoginTries { get; set; }

        #region Constructors
        static UserManager()
        {
            UserDatabase = new DatabaseStuff();
        }

        public UserManager(string userName, string password, bool isNewUser = false)
        {
            LoginTries = 1;
            if (IsWrongUserName(userName, isNewUser))
            {
                AskUserName(isNewUser);
            }
            else
            {
                UserName = userName;
            }
            LoginTries = 1;
            if (IsWrongPassword(password, isNewUser))
            {
                AskPassword(isNewUser);
            }
            else
            {
                Password = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
            }
            SetAccessibility(isNewUser);
            // If is new user create a user, else get the user from database
            TheUser = (isNewUser)?new User(UserName, Password, UserAccess): UserDatabase.Users.Single(u => u.UserName == UserName);
        }

        public UserManager(bool isNewUser = false) : this("", "", isNewUser) { }
        #endregion

        #region UserName Methods
        private void AskUserName(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tUser Name:\t");
            string userName = Console.ReadLine();
            while (IsWrongUserName(userName, isNewUser) && LoginTries < 3)
            {
                Console.Write("\n\n\tUser Name:\t");
                userName = Console.ReadLine();
                LoginTries++;   
            }
            if (LoginTries == 3 && IsWrongUserName(userName, isNewUser))
            {
                throw new ArgumentException("\n\n\tToo many false attempts");
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
            else if (userName.Contains(' '))
            {
                Console.Write("\n\n\tUser Name cannot contain spaces! Try again");
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
        #endregion

        #region Password Methods
        private void AskPassword(bool isNewUser)
        {
            Console.Clear();
            Console.Write("\n\n\tPassword:\t");
            string password = PasswordHandling.GetPassword();
            while (IsWrongPassword(password, isNewUser) && LoginTries < 3)
            {
                Console.Write("\n\n\tPassword:\t");
                password = PasswordHandling.GetPassword();
                LoginTries++;
            }
            if (LoginTries == 3 && IsWrongPassword(password, isNewUser))
            {
                throw new ArgumentException("\n\n\tToo many false attempts");
            }
            Password = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
        }

        private bool IsWrongPassword(string password, bool isNewUser)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            if (password.Length < 5 || password.Length > 20)
            {
                Console.Write("\n\n\tPassword has to be between 5 and 20 characters long!");
                Console.ResetColor();
                return true;
            }
            else if (password.Contains(' '))
            {
                Console.Write("\n\n\tPassword cannot contain spaces! Try again");
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

        private bool IDmatched(string password)
        {
            string passHash = UserDatabase.Users.Single(i => i.UserName == UserName).Password;
            string givenPass = PasswordHandling.PasswordToHash(PasswordHandling.ConvertToSecureString(password), UserName);
            return (givenPass == passHash);
        }
        #endregion

        private void SetAccessibility(bool isNewUser)
        {
            if (isNewUser)
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
            else
            {
                UserAccess = UserDatabase.Users.Single(u => u.UserName == UserName).UserAccess;
            }
            
        }

    }
}
