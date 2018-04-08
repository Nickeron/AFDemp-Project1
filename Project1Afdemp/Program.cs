using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    try
                    {
                        UserManager newAdmin = new UserManager("admin", "aDmI3$", true);
                        UserManager newGuest = new UserManager("guest", "guest", true);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }
            // 
            UserManager activeUserManager;
            while (true)
            {
                activeUserManager = LoginScreen();
                MainMenu(activeUserManager);
            }
        }

        public static UserManager LoginScreen()
        {
            UserManager activeUserManager;
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
                            activeUserManager = new UserManager();
                            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUserManager.UserAccess} {activeUserManager.UserName}");
                            Thread.Sleep(1200);
                            return activeUserManager;
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
                            activeUserManager = new UserManager(true);
                            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUserManager.UserAccess} {activeUserManager.UserName}");
                            Thread.Sleep(1200);
                            return activeUserManager;
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

        public static void MainMenu(UserManager activeUserManager)
        {
            while (true)
            {
                using (var database = new DatabaseStuff())
                {
                    int unreadMessages = database.Messages.Count(m => m.IsRead == false && m.Receiver.Id == activeUserManager.TheUser.Id);
                    int unreadChat = database.Users.Single(c=> c.UserName==activeUserManager.UserName).IdsUnreadChatMessages.Split(' ').Length-1;

                    List<string> mainMenuItems = new List<string> { $"Chat ({unreadChat})", "Send Email", $"Read Received ({unreadMessages})", "Transaction History", "Log Out", "Exit" };

                    if (activeUserManager.UserAccess == Accessibility.administrator)
                    {
                        mainMenuItems.Insert(3, "Manage Users");
                        mainMenuItems.Insert(4, "Create NEW User");
                    }

                    string userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);

                    if (userChoice.Contains("Chat"))
                    {
                        ShowChat(activeUserManager);
                    }
                    else if (userChoice.Contains("Send Email"))
                    {
                        SendEmail(activeUserManager);
                    }
                    else if (userChoice.Contains("Read Received"))
                    {
                        ReadReceived(activeUserManager);
                    }
                    else if (userChoice.Contains("Transaction History"))
                    {
                        TransactionHistory(activeUserManager);
                    }
                    else if (userChoice.Contains("Manage Users"))
                    {
                        ManageUsers(activeUserManager);
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

        public static void ShowChat(UserManager activeUserManager)
        {
            while (true)
            {
                Console.Clear();
                string chat = StringsFormatted.Chat + "\n\n";
                using (var database = new DatabaseStuff())
                {
                    var chatMessages = database.Chat.OrderBy(i => i.Id);
                    foreach (var message in chatMessages)
                    {
                        if (message.Id.ToString().Equals(database.Users.Single(c => c.UserName == activeUserManager.UserName).IdsUnreadChatMessages.Split(' ').FirstOrDefault()))
                        {
                            chat += "\n\t__________________________NEW__________________________\n";
                        }
                        Debug.WriteLine(database.Users.Single(c => c.UserName == activeUserManager.UserName).IdsUnreadChatMessages.Split(' ').FirstOrDefault());
                        chat += "\n\t" + message.TimeSent.ToString("MM/dd HH:mm") + ' ' +
                            (database.Users.Single(i => i.Id == message.SenderId).UserName.ToString() + ":").PadRight(15) +
                            message.Text + '\n';
                    }
                    activeUserManager.ClearUnreadChat();
                    if (Menus.HorizontalMenu(chat, new List<string> { "Reply", "Back" }).Contains("Back"))
                    {
                        break;
                    }
                    Console.Clear();
                    Console.Write(chat + "\n\n\t" + activeUserManager.UserName + ": ");
                    ChatMessage newReply = new ChatMessage(activeUserManager.TheUser.Id, Console.ReadLine());
                    database.Chat.Add(newReply);
                    database.SaveChanges();
                    ChatMessage freshReply = database.Chat.OrderByDescending(t => t.TimeSent).First();
                    var unReadUsers = database.Users.Where(u => u.UserName != activeUserManager.UserName);
                    foreach (User unreadUser in unReadUsers)
                    {
                        unreadUser.IdsUnreadChatMessages += freshReply.Id.ToString()+' ';
                    }
                    database.SaveChanges();
                }
            }
        }

            public static void SendEmail(UserManager activeUserManager)
        {
            User receiver = SelectUser(activeUserManager);
            if (receiver is null) { return; }
            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write("\n\n\tTitle: ");
            string MessageTitle = Console.ReadLine();

            Console.Write("\n\tBody: ");
            string MessageBody = Console.ReadLine();

            using (var database = new DatabaseStuff())
            {
                int senderId = database.Users.Single(i => i.UserName == activeUserManager.UserName).Id;
                Message email = new Message(senderId, receiver.Id, MessageTitle, MessageBody);
                try
                {
                    database.Messages.Add(email);
                    database.SaveChanges();
                    Console.Write($"\n\n\tEmail sent successfully to {receiver.UserName}\n\n\tOK");
                }
                catch (Exception e) { Console.WriteLine(e); }
                Console.ReadKey();
            }
        }

        public static void ReadReceived(UserManager activeUserManager)
        {
            Message receivedMessage = SelectMessage(activeUserManager);
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

        public static void TransactionHistory(UserManager activeUserManager)
        {
            Console.Clear();
            Console.WriteLine(StringsFormatted.History+'\n');
            using (var database = new DatabaseStuff())
            {
                List<Message> messages = database.Messages.ToList();
                int receiverId = database.Users.Single(i => i.UserName == activeUserManager.UserName).Id;
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

        public static void ManageUsers(UserManager activeUserManager)
        {
            User receiver = SelectUser(activeUserManager);
            if (receiver is null) { return; }
            List<string> ChangeUserMenuItems = new List<string> { "Permissions", "Delete User", "Back" };
            string manageUsersChoice = Menus.VerticalMenu(StringsFormatted.ManageUsers, ChangeUserMenuItems);
            if(manageUsersChoice == "Delete User")
            {
                DeleteUser(receiver);
            }
            else if (manageUsersChoice == "Permissions")
            {
                ChangeUserPermissions(receiver);
            }
        }

        public static void CreateNewUser()
        {
            using (var database = new DatabaseStuff())
            {
                UserManager newUser = new UserManager(true);
            }
        }

        public static User SelectUser(UserManager activeUserManager)
        {
            List<string> selectUserItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<User> availableUsers = database.Users.ToList();

                try
                {
                    foreach (User user in availableUsers)
                    {
                        if (user.UserName != activeUserManager.UserName)
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
                string selectedUserName = Menus.VerticalMenu(StringsFormatted.SelectUser, selectUserItems);

                Console.Clear();
                return database.Users.Single(i => i.UserName == selectedUserName);
            }
        }

        public static Message SelectMessage(UserManager activeUserManager)
        {
            List<string> selectMessageItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<Message> messages = database.Messages.ToList();
                int receiverId = database.Users.Single(i => i.UserName == activeUserManager.UserName).Id;
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

        public static void DeleteUser(User deletingUser)
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete this user?", new List<string>{ "Yes","No"}).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    database.Users.Remove(database.Users.Single(i => i.UserName == deletingUser.UserName));
                    var deletingMessages = database.Messages.Where(i => i.ReceiverId == deletingUser.Id || i.SenderId == deletingUser.Id);
                    foreach(Message deletingMessage in deletingMessages)
                    {
                        database.Messages.Remove(deletingMessage);
                    }
                    database.SaveChanges();                       
                }
            }
        }

        public static void ChangeUserPermissions(User changingUser)
        {
            List<string> manageUserItems;
            if (changingUser.UserAccess == Accessibility.administrator)
            {
                manageUserItems = new List<string>() { "downgrade to USER", "downgrade to GUEST" ,"Back"};
            }
            else if(changingUser.UserAccess == Accessibility.user)
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "downgrade to GUEST", "Back" };
            }
            else
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "upgrade to USER", "Back" };
            }
            string changeOfAccess = Menus.VerticalMenu($"\n\n\t{changingUser.UserName} is {changingUser.UserAccess}, how do you want to change his permissions?", manageUserItems);
            using (var database = new DatabaseStuff())
            {
                User changedUser = database.Users.Single(i => i.UserName == changingUser.UserName);
                if (changeOfAccess.Contains("ADMINISTRATOR"))
                {
                    changedUser.UserAccess = Accessibility.administrator;
                }
                else if (changeOfAccess.Contains("USER"))
                {
                    changedUser.UserAccess = Accessibility.user;
                }
                else if (changeOfAccess.Contains("GUEST"))
                {
                    changedUser.UserAccess = Accessibility.guest;
                }
                else
                {
                    return;
                }
                database.SaveChanges();
            }
            Console.Write($"\n\n\tYou did {changeOfAccess}, the user: {changingUser.UserName}\n\n\tOK");
            Console.ReadKey();
        }
    }
}
