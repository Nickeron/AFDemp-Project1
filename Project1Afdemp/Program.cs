using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Data.Entity;

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
                using (var database = new DatabaseStuff())
                {
                    try
                    {
                        database.Users.Add(activeUser);
                        database.SaveChanges();
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                    
                }
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
                case 0:
                    {
                        SendEmail(activeUser);
                        break;
                    }
                case 1:
                    {
                        ReadReceived(activeUser);
                        break;
                    }
                case 2:
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
            using (var database = new DatabaseStuff())
            {
                // Display all Blogs from the database 
                var query = from user in database.Users
                            orderby user.UserName
                            select user;

                Console.WriteLine("All users in the database:");
                foreach (User user in query)
                {
                    Console.WriteLine(user.UserName);
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            string[] sendEmailItems = new string[] { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.SendEmail, sendEmailItems);
        }

        public static void ReadReceived(User activeUser)
        {
            string[] readEmailItems = new string[] { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.ReadEmails, readEmailItems);
        }

        public static void TransactionHistory(User activeUser)
        {
            string[] historyItems = new string[] { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.History, historyItems);
        }

        public static void ManageUsers(User activeUser)
        {
            string[] manageUsersItems = new string[] { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.ManageUsers, manageUsersItems);
        }



    }
}
