using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Project1Afdemp
{
    class Program
    {

        static void Main(string[] args)
        {
            // If database is empty create the 2 basic users beforehand.
            using (var database = new DatabaseStuff())
            {
                if (database.Users.Count().Equals(0))
                {
                    UserManager newAdmin = new UserManager("admin", "aDmI3$", true);
                    UserManager newGuest = new UserManager("guest", "guest", true);
                    try
                    {
                        database.Users.Add(newAdmin.TheUser);
                        database.Users.Add(newGuest.TheUser);
                        database.SaveChanges();
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }

            UserManager activeUser;
            while (true)
            {
                activeUser = LoginScreen();
                MainMenu(activeUser);
            }
        }

        public static UserManager LoginScreen()
        {
            UserManager activeUser;
            while (true)
            {
                List<string> signOrLogItems = new List<string> { "Sign Up", "Log In" };
                string userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);
                using (var database = new DatabaseStuff())
                {
                    if (userChoice == "Log In")
                    {
                        try
                        {
                            activeUser = new UserManager();
                            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUser.UserAccess} {activeUser.UserName}");
                            Thread.Sleep(1200);
                            return activeUser;
                        }
                        catch (Exception e)
                        {
                            Console.Clear();
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine(e.Message);
                            Thread.Sleep(1200);
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        try
                        {
                            activeUser = new UserManager(true);
                            database.Users.Add(activeUser.TheUser);
                            database.SaveChanges();
                            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUser.UserAccess} {activeUser.UserName}");
                            Thread.Sleep(1200);
                            return activeUser;
                        }
                        catch (Exception e)
                        {
                            Console.Clear();
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine(e.Message);
                            Thread.Sleep(1200);
                            Console.ResetColor();
                        }
                    }
                }
            }
        }

        public static void MainMenu(UserManager activeUser)
        {
            while (true)
            {
                using (var database = new DatabaseStuff())
                {
                    int unreadMessages = database.Messages.Count(m => m.IsRead == false && m.Receiver.Id == activeUser.TheUser.Id);
                    List<string> mainMenuItems = new List<string> { "Send Email", $"Read Received ({unreadMessages})", "Transaction History", "Log Out", "Exit" };

                    if (activeUser.UserAccess == Accessibility.administrator)
                    {
                        mainMenuItems.Insert(3, "Manage Users");
                        mainMenuItems.Insert(4, "Create NEW User");
                    }

                    string userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);

                    if (userChoice.Contains("Send Email"))
                    {
                        SendEmail(activeUser);
                    }
                    else if (userChoice.Contains("Read Received"))
                    {
                        ReadReceived(activeUser);

                    }
                    else if (userChoice.Contains("Transaction History"))
                    {
                        TransactionHistory(activeUser);
                    }
                    else if (userChoice.Contains("Manage Users"))
                    {
                        ManageUsers(activeUser);
                    }
                    else if (userChoice.Contains("Create NEW User"))
                    {
                        CreateNewUser();
                    }
                    else if (userChoice.Contains("Log Out"))
                    {
                        return;
                    }
                    else if (userChoice.Contains("Exit"))
                    {
                        Environment.Exit(0);
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
            using (var database = new DatabaseStuff())
            {
                Message readMessage = database.Messages.Single(m => m.Id == receivedMessage.Id);
                readMessage.IsRead = true;
                database.SaveChanges();
            }
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
            List<string> ChangeUserItems = new List<string> { "Permissions", "Delete User", "Back" };
            string choice = Menus.VerticalMenu(StringsFormatted.ManageUsers, ChangeUserItems);
            if(choice == "Delete User")
            {
                DeleteUser(receiver);
            }
            else if (choice == "Permissions")
            {
                ChangeUserPermissions(receiver);
            }
        }

        public static void CreateNewUser()
        {
            using (var database = new DatabaseStuff())
            {
                UserManager newUser = new UserManager(true);
                try
                {
                    database.Users.Add(newUser.TheUser);
                    database.SaveChanges();
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
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
                    string listedMessage;
                    foreach (Message message in messages)
                    {
                        if (message.ReceiverId == receiverId)
                        {
                            senderName = database.Users.Single(i => i.Id == message.SenderId).UserName;
                            if (!message.IsRead)
                            {
                                listedMessage = "* ";
                            }
                            else
                            {
                                listedMessage = "";
                            }
                            listedMessage += $"ID: |{message.Id}| From: |{senderName}| Title: |{message.Title}| Time Sent: |{message.TimeSent}|";
                            selectMessageItems.Add(listedMessage);
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
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete this user?", new List<string>{ "Yes","No"}).Contains('Y'))
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

        public static void ChangeUserPermissions(User chUser)
        {
            List<string> manageUserItems;
            if (chUser.UserAccess == Accessibility.administrator)
            {
                manageUserItems = new List<string>() { "downgrade to USER", "downgrade to GUEST" ,"Back"};
            }
            else if(chUser.UserAccess == Accessibility.user)
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "downgrade to GUEST", "Back" };
            }
            else
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "upgrade to USER", "Back" };
            }
            string change = Menus.VerticalMenu($"\n\n\t{chUser.UserName} is {chUser.UserAccess}, how do you want to change his permissions?", manageUserItems);
            using (var database = new DatabaseStuff())
            {
                User toChange = database.Users.Single(i => i.UserName == chUser.UserName);
                if (change.Contains("ADMINISTRATOR"))
                {
                    toChange.UserAccess = Accessibility.administrator;
                }
                else if (change.Contains("USER"))
                {
                    toChange.UserAccess = Accessibility.user;
                }
                else if (change.Contains("GUEST"))
                {
                    toChange.UserAccess = Accessibility.guest;
                }
                else
                {
                    return;
                }
                database.SaveChanges();
            }
            Console.Write($"\n\n\tYou did {change}, the user: {chUser.UserName}\n\n\tOK");
            Console.ReadKey();
        }
    }
}
