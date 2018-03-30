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
        }

        public static UserManager LoginScreen()
        {
            UserManager activeUser;
            List<string> signOrLogItems = new List<string>{ "Sign Up", "Log In" };
            string userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);
            using (var database = new DatabaseStuff())
            {
                if (userChoice == "Log In")
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
            List<string> mainMenuItems = new List<string> { "Send Email", "Read Received", "Transaction History", "Exit" };
            if (activeUser.UserAccess == Accessibility.administrator)
            {
                mainMenuItems.Insert(3, "Manage Users");
            }
            while (true)
            {
                string userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);

                switch (userChoice)
                {
                    case "Send Email":
                        {
                            SendEmail(activeUser);
                            break;
                        }
                    case "Read Received":
                        {
                            ReadReceived(activeUser);
                            break;
                        }
                    case "Transaction History":
                        {
                            TransactionHistory(activeUser);
                            break;
                        }
                    case "Manage Users":
                        {
                            ManageUsers(activeUser);
                            break;
                        }
                    case "Exit":
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            } 
        }

        public static void SendEmail(UserManager activeUser)
        {
            User receiver = SelectUser(activeUser);

            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write("\n\n\tTitle: ");
            string MessageTitle = Console.ReadLine();

            Console.Write("\n\tBody: ");
            string MessageBody = Console.ReadLine();

            using (var database = new DatabaseStuff())
            {
                Message email = new Message(activeUser.TheUser.Id, receiver.Id, MessageTitle, MessageBody);
                try
                {
                    database.Messages.Add(email);
                    Console.WriteLine("Adding the email SUCCESS!!!!" + receiver.UserName);
                    database.SaveChangesAsync();
                    Console.WriteLine("Email sent successfully to" + receiver.UserName);
                }
                catch (Exception e) { Console.WriteLine(e); }
                Console.ReadKey();
            }
        }

        public static void ReadReceived(UserManager activeUser)
        {
            List<string> readEmailItems = new List<string> ();

            string userChoice = Menus.VerticalMenu(StringsFormatted.ReadEmails, readEmailItems);
        }

        public static void TransactionHistory(UserManager activeUser)
        {
            List<string> historyItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
            string userChoice = Menus.VerticalMenu(StringsFormatted.History, historyItems);
        }

        public static void ManageUsers(UserManager activeUser)
        {
            List<string> manageUsersItems = new List<string> { "Send Email", "Read Received", "Transaction History" };
            string userChoice = Menus.VerticalMenu(StringsFormatted.ManageUsers, manageUsersItems);
        }

        public static User SelectUser(UserManager activeUser)
        {
            List<string> selectUserItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<User> users = database.Users.ToList();

                try
                {
                    foreach (User user in users)
                    {
                        if (user.UserName != activeUser.UserName)
                        {
                            selectUserItems.Add(user.UserName);
                        }                        
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                string sUser = Menus.VerticalMenu(StringsFormatted.SelectUser, selectUserItems);

                Console.Clear();
                return database.Users.Single(i => i.UserName == sUser);
            }
        }
    }
}
