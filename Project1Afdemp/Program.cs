using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Project1Afdemp
{
    class Program
    {
        static void Main(string[] args)
        {

            User activeUser = LoginScreen();
            MainMenu(activeUser);


            Console.ReadKey();
        }

        public static User LoginScreen()
        {
            User activeUser;
            string[] signOrLogItems = { "Sign Up", "Log In" };
            short userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);

            if (userChoice == 1)
            {
                activeUser = new User("", new SecureString());
            }
            else
            {
                activeUser = new User("", new SecureString(), true);
            }
            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUser.UserName}");
            return activeUser;
        }

        public static void MainMenu(User activeUser)
        {
            string[] mainMenuItems;
            if (activeUser.UserAccess == Accessibility.administrator)
            {
                mainMenuItems = new string[] { "Send Email", "Read Received", "Transaction History" , "Manage Users"};
            }
            else
            {
                mainMenuItems = new string[] { "Send Email", "Read Received", "Transaction History" };
            }
            
            short userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);

            switch (userChoice)
            {
                case 1:
                    {
                        SendEmail(activeUser);
                        break;
                    }
                case 2:
                    {
                        ReadReceived(activeUser);
                        break;
                    }
                case 3:
                    {
                        TransactionHistory(activeUser);
                        break;
                    }
                default:
                    {
                        ManageUsers(activeUser);
                        break;
                    }
            }
        }

        public static void SendEmail(User activeUser)
        {

        }

        public static void ReadReceived(User activeUser)
        {

        }

        public static void TransactionHistory(User activeUser)
        {

        }

        public static void ManageUsers(User activeUser)
        {

        }



    }
}
