using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Project1Afdemp
{
    class MenuFunctions
    {
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
                        chat += "\n\t" + message.TimeSent.ToString("MM/dd HH:mm") + "   " +
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
                        unreadUser.IdsUnreadChatMessages += freshReply.Id.ToString() + ' ';
                    }
                    database.SaveChanges();
                }
            }
        }

        public static void SendEmail(UserManager activeUserManager)
        {
            User receiver = SideFunctions.SelectUser(activeUserManager);
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
        
        public static void SelectMessageAction(UserManager activeUserManager)
        {
            string userChoice;
            do
            {
                Message receivedMessage = SideFunctions.SelectMessage(activeUserManager);
                if (receivedMessage is null) { return; }
                List<string> messageOptions = new List<string> { "Read", "Delete", "Back" };
                userChoice = Menus.VerticalMenu(StringsFormatted.Options, messageOptions);
                if (userChoice.Contains("Read"))
                {
                    SideFunctions.ReadReceivedMessage(receivedMessage);
                }
                else if (userChoice.Contains("Delete"))
                {
                    SideFunctions.DeleteMessage(receivedMessage);
                }
            }
            while (!userChoice.Contains("Back"));
        }

        public static void TransactionHistory(UserManager activeUserManager)
        {
            Console.Clear();
            Console.WriteLine(StringsFormatted.History + '\n');
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
            List<string> ChangeUserMenuItems = new List<string> { "Permissions", "Delete User", "Create a NEW User", "Back" };
            string manageUsersChoice = Menus.VerticalMenu(StringsFormatted.ManageUsers, ChangeUserMenuItems);
            if (manageUsersChoice.Contains("Create"))
            {
                SideFunctions.CreateNewUser();
                return;
            }
            else if (manageUsersChoice.Contains("Back"))
            {
                return;
            }
            User receiver = SideFunctions.SelectUser(activeUserManager);
            if (receiver is null) { return; }
            if (manageUsersChoice == "Delete User")
            {
                SideFunctions.DeleteUser(receiver);
            }
            else if (manageUsersChoice == "Permissions")
            {
                SideFunctions.ChangeUserPermissions(receiver);
            }
        }
    }
}
