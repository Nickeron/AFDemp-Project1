using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Project1Afdemp
{
    static class MenuFunctions
    {
        public static UserManager LoginScreen()
        {
            while (true)
            {
                string userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, new List<string> { "Sign Up", "Log In" });
                using (var database = new DatabaseStuff())
                {
                    if (userChoice == "Log In")
                    {
                        try
                        { return SuccessfullLogin(isNewUser: false); }
                        catch (Exception e) { PrintException(e); }
                    }
                    else
                    {
                        try
                        { return SuccessfullLogin(isNewUser: true); }
                        catch (Exception e)  { PrintException(e); }
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
                    // Access the active user from Database
                    User activeUser = database.Users.Single(u => u.UserName == activeUserManager.UserName);

                    // Get all the chat messages ordered increasingly and 
                    // access the users haven't read them yet.
                    var chatMessages = database.Chat.Include("UnreadUsers").OrderBy(i => i.Id);

                    bool firstNewMessage = true;

                    foreach (var message in chatMessages)
                    {
                        if (message.UnreadUsers.Contains(activeUser) && firstNewMessage)
                        {
                            chat += "\n\t__________________________NEW__________________________\n";
                            firstNewMessage = false;
                        }
                        chat += "\n\t" + message.TimeSent.ToString("MM/dd HH:mm") + "   " +
                            (database.Users.Single(i => i.Id == message.SenderId).UserName.ToString() 
                            + ":").PadRight(15) + message.Text + '\n';
                    }
                    // Since by this point the user read the new messages
                    activeUserManager.ClearUnreadChat();

                    // Does the user wish to reply?
                    if (Menus.HorizontalMenu(chat, new List<string> { "Reply", "Back" }).Contains("Back"))
                    { break; }

                    // Rewrite the whole chat with the username added at the bottom
                    Console.Clear();
                    Console.Write(chat + "\n\n\t" + activeUser.UserName + ": ");
 
                    // Collect all the other users in a list
                    var unreadUsers = database.Users.Where(u => u.UserName != activeUser.UserName).ToList();

                    // Create the new chat message
                    database.Chat.Add(new ChatMessage
                    {
                        Sender = activeUser,
                        Text = Console.ReadLine(),
                        UnreadUsers = unreadUsers,
                        TimeSent = DateTime.Now
                    });
                    database.SaveChanges();
                }
            }
        }

        public static void SendEmail(UserManager activeUserManager, User receiver = null)
        {
            string title;
            if(receiver is null)
            {
                title = "\n\n\tTitle: ";
                receiver = SideFunctions.SelectUser(activeUserManager);
            }
            else
            {
                title = "\n\n\tTitle: RE>";
            }
            if (receiver is null) { return; }
            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write(title);
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
                catch (Exception e) { PrintException(e); }
                Console.ReadKey();
            }
        }
        
        public static void SelectMessageAction(UserManager activeUserManager, bool Received = true)
        {
            string userChoice;
            do
            {
                Message selectedMessage = SideFunctions.SelectMessage(activeUserManager, Received);
                if (selectedMessage is null) { return; }
                string presentedMessage = StringsFormatted.ReadEmails + $"\n\n\tTitle: {selectedMessage.Title}" +
                    $"\n\n\tBody: {selectedMessage.Body}\n\n";
                List<string> messageOptions = new List<string> { "Forward", "Delete", "Back" };
                if (Received)
                { messageOptions.Insert(1, "Reply"); }
                userChoice = Menus.HorizontalMenu(presentedMessage, messageOptions);
                using (var database = new DatabaseStuff())
                {
                    Message readMessage = database.Messages.Single(m => m.Id == selectedMessage.Id);
                    readMessage.IsRead = true;
                    database.SaveChanges();
                    if (userChoice.Contains("Forward"))
                    {
                        SideFunctions.ForwardMessage(activeUserManager, selectedMessage);
                    }
                    else if (userChoice.Contains("Reply"))
                    {

                        SendEmail(activeUserManager, database.Users.Single(u => u.Id == readMessage.Sender.Id));
                    }
                    else if (userChoice.Contains("Delete"))
                    {
                        SideFunctions.DeleteMessage(selectedMessage);
                    }
                }
            }
            while (!userChoice.Contains("Back"));
        }

        public static void CommunicationHistory(UserManager activeUserManager)
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
                        Console.WriteLine($"\t{message.TimeSent} {senderName} sent '{message.Title}' to {receiverName}");
                    }
                }
                catch (Exception e) { PrintException(e); }
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

        private static UserManager SuccessfullLogin(bool isNewUser)
        {
            UserManager activeUserManager = new UserManager(isNewUser);
            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUserManager.UserAccess} {activeUserManager.UserName}");
            Thread.Sleep(1200);
            return activeUserManager;
        }

        private static void PrintException(Exception e)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message + "\n\n\tBack");
            Console.ReadKey();
            Console.ResetColor();
        }
    }
}
