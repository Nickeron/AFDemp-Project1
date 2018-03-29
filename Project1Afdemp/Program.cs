using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    class Program
    {
        static void Main(string[] args)
        {
            UserManager activeUser = LoginScreen();
            MainMenu(activeUser);


            Console.ReadKey();
        }

        public static UserManager LoginScreen()
        {
            UserManager activeUser;
            List<string> signOrLogItems = new List<string>{ "Sign Up", "Log In" };
            short userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);
            using (var database = new DatabaseStuff())
            {
                if (userChoice == 1)
                {
                    activeUser = new UserManager();
                }
                else
                {
                    activeUser = new UserManager(true);
                    try
                    {
                        database.Users.Add(activeUser.TheUser);
                        database.SaveChanges();
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }
            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUser.UserName}");
            return activeUser;
        }

        public static void MainMenu(UserManager activeUser)
        {
            List<string> mainMenuItems;
            if (activeUser.UserAccess == Accessibility.administrator)
            {
                mainMenuItems = new List<string> { "Send Email", "Read Received", "Transaction History" , "Manage Users"};
            }
            else
            {
                mainMenuItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
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

        public static void SendEmail(UserManager activeUser)
        {
            List<string> sendEmailItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                var query = from user in database.Users
                            orderby user.UserName
                            select user;
                try
                {
                    foreach (User user in query)
                    {
                        sendEmailItems.Add(user.UserName);
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
            sendEmailItems.Add("back");
            short userChoice = Menus.VerticalMenu(StringsFormatted.SendEmail, sendEmailItems);
        }

        public static void ReadReceived(UserManager activeUser)
        {
            List<string> readEmailItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.ReadEmails, readEmailItems);
        }

        public static void TransactionHistory(UserManager activeUser)
        {
            List<string> historyItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.History, historyItems);
        }

        public static void ManageUsers(UserManager activeUser)
        {
            List<string> manageUsersItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
            short userChoice = Menus.VerticalMenu(StringsFormatted.ManageUsers, manageUsersItems);
        }



    }
}
