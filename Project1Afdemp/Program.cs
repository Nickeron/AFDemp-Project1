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
            if (receiver is null) { return; }
            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write("\n\n\tTitle: ");
            string MessageTitle = Console.ReadLine();

            Console.Write("\n\tBody: ");
            string MessageBody = Console.ReadLine();

            using (var database = new DatabaseStuff())
            {
                int senderId = database.Users.Single(i => i.UserName == activeUser.UserName).Id;
                Message email = new Message(senderId, receiver.Id, MessageTitle, MessageBody);
                try
                {
                    database.Messages.Add(email);
                    database.SaveChanges();
                    Console.Write($"\n\n\tEmail sent successfully to {receiver.UserName}\n\tOK");
                }
                catch (Exception e) { Console.WriteLine(e); }
                Console.ReadKey();
            }
        }

        public static void ReadReceived(UserManager activeUser)
        {
            Message receivedMessage = SelectMessage(activeUser);
            if (receivedMessage is null) { return; }
            Console.Write($"\n\n\tTitle: {receivedMessage.Title}\n\n\tBody: {receivedMessage.Body}\n\n\tOK");
            Console.ReadKey();
        }

        public static void TransactionHistory(UserManager activeUser)
        {
            Console.Clear();
            Console.WriteLine(StringsFormatted.History+'\n');
            using (var database = new DatabaseStuff())
            {
                List<Message> messages = database.Messages.ToList();
                int receiverId = database.Users.Single(i => i.UserName == activeUser.UserName).Id;
                string senderName;
                string receiverName;
                try
                {
                    foreach (Message message in messages)
                    {
                        senderName = database.Users.Single(i => i.Id == message.SenderId).UserName;
                        receiverName = database.Users.Single(i => i.Id == message.ReceiverId).UserName;
                        Console.WriteLine($"\tAt {message.TimeSent}, {senderName} sent a mail to {receiverName} with a title: '{message.Title}'");
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                Console.Write("\n\n\tOK");
                Console.ReadKey();
            }
        }

        public static void ManageUsers(UserManager activeUser)
        {
            User receiver = SelectUser(activeUser);
            if (receiver is null) { return; }
            List<string> ChangeUserItems = new List<string> { "Permissions", "Delete User" };
            string choice = Menus.VerticalMenu(StringsFormatted.ManageUsers, ChangeUserItems);
            if(choice == "Delete User")
            {
                DeleteUser(receiver);
                return;
            }
            //ChangeUserPermissions(receiver);
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
                if (selectUserItems.Count == 0)
                {
                    Console.WriteLine("\n\n\tNo users in database!");
                    Console.ReadKey();
                    return null;
                }
                string sUser = Menus.VerticalMenu(StringsFormatted.SelectUser, selectUserItems);

                Console.Clear();
                return database.Users.Single(i => i.UserName == sUser);
            }
        }

        public static Message SelectMessage(UserManager activeUser)
        {
            List<string> selectMessageItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<Message> messages = database.Messages.ToList();
                int receiverId = database.Users.Single(i => i.UserName == activeUser.UserName).Id;
                string senderName;
                try
                {
                    foreach (Message message in messages)
                    {
                        if (message.ReceiverId == receiverId)
                        {
                            senderName = database.Users.Single(i => i.Id == message.SenderId).UserName;
                            selectMessageItems.Add($"ID: |{message.Id}| From: |{senderName}| Title: |{message.Title}| Time Sent: |{message.TimeSent}|");
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }

                if(selectMessageItems.Count == 0)
                {
                    Console.WriteLine("\n\n\tNo Messages to View");
                    Console.ReadKey();
                    return null;
                }
                string oMessage = Menus.VerticalMenu(StringsFormatted.OpenMessage, selectMessageItems);
                string[] selParameters = oMessage.Split('|');
                int messageID = int.Parse(selParameters[1]);

                Console.Clear();
                return database.Messages.Single(i => i.Id == messageID);
            }
        }

        public static void DeleteUser(User delUser)
        {
            if (Menus.HorizontalMenu("Are you sure you want to delete this user?", new List<string>{ "Yes","No"}).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    database.Users.Remove(database.Users.Single(i => i.UserName == delUser.UserName));
                    var query = database.Messages.Where(i => i.ReceiverId == delUser.Id || i.SenderId == delUser.Id);
                    foreach(var item in query)
                    {
                        database.Messages.Remove(item);
                    }
                    database.SaveChanges();                       
                }
            }
        }

    }
}
