using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;


namespace Project1Afdemp
{
    [Table("Users")]
    public class User
    {
        private static Dictionary<string, SecureString> UsersList { get; set; }
        [Key]
        public string UserName { get; set; }
        public SecureString Password { get; set; }
        public Accessibility UserAccess { get; private set; }

        static User()
        {
            UsersList = new Dictionary<string, SecureString>
            {
                { "admin", PasswordHandling.ConvertToSecureString("aDmI3$") },
                { "nicker", PasswordHandling.ConvertToSecureString("nicker") },
                { "paspam", PasswordHandling.ConvertToSecureString("paspam") },
                { "paspar", PasswordHandling.ConvertToSecureString("paspar") },
                { "paspa", PasswordHandling.ConvertToSecureString("paspa") },
                { "johny", PasswordHandling.ConvertToSecureString("johny") }
            };
        }

        public User(string  userName, SecureString password, bool isNewUser = false)
        {
            if (IsWrongUserName(userName, isNewUser))
            {
                AskUserName(isNewUser);
            }
            else
            {
                UserName = userName;
            }
            if (UserName!="guest" || IsWrongPassword(password, isNewUser))
            {
                AskPassword(isNewUser);
            }
            else
            {
                Password = password;
            }
            SetAccessibility();
        }

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
            else if(UserNameAlreadyExists(userName) && isNewUser)
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
            return UsersList.ContainsKey(userName);
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
            Password = password;
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
            return (PasswordHandling.SecureCompare(UsersList[UserName], password));
        }

        private void SetAccessibility()
        {
            if(UserName == "admin")
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
