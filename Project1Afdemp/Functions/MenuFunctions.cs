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
                        catch (Exception e) { PrintException(e); }
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
                        chat += "\n\t" + message.TimeSent.ToString("dd/MM HH:mm") + "   " +
                            (database.Users.Single(i => i.Id == message.SenderId).UserName.ToString()
                            + ":").PadRight(15) + message.Text + '\n';
                    }
                    // Since by the next point the user read the new messages
                    activeUserManager.ClearUnreadChat();


                    List<string> chatOptions = new List<string> { "Reply", "Back" };
                    if (activeUser.UserAccess == Accessibility.administrator && database.Chat.Any())
                    {
                        chatOptions.Insert(1, "Edit");
                        chatOptions.Insert(2, "Delete All");
                    }
                    string userChoice = Menus.HorizontalMenu(chat, chatOptions);

                    // Does the user wish to leave?
                    if (userChoice.Contains("Back"))
                    { break; }

                    // Or edit the chat Messages
                    else if (userChoice.Contains("Edit"))
                    {
                        ChatFunctions.EditChatMessages();
                    }
                    // Maybe delete them all?
                    else if (userChoice.Contains("Delete All"))
                    {
                        ChatFunctions.DeleteAllChatMessages();
                    }
                    // Or add a reply
                    else
                    {
                        ChatFunctions.AddReplyToChat(activeUserManager, chat);
                    }
                }
            }
        }

        public static void SendEmail(UserManager activeUserManager, User receiver = null, string reTitle = "")
        {
            string title;
            if (receiver is null)
            {
                title = "\n\n\tTitle: ";
                receiver = SideFunctions.SelectUser(activeUserManager);
            }
            else
            {
                title = "\n\n\tTitle: RE> " + reTitle;
            }
            if (receiver is null) { return; }
            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write(title);
            string MessageTitle = (receiver is null) ? Console.ReadLine() : "RE> " + reTitle;

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

        public static void PresentAndManipulateMessage(UserManager activeUserManager, bool Received = true)
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
                { messageOptions.Insert(0, "Reply"); }
                else
                { messageOptions.Insert(0, "Edit"); }

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
                        User toBeReplied = database.Users.Single(u => u.Id == readMessage.Sender.Id);
                        SendEmail(activeUserManager, toBeReplied, readMessage.Title);
                    }
                    else if (userChoice.Contains("Edit"))
                    {
                        SideFunctions.UpdateEmail(activeUserManager, selectedMessage);
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

        public static void PrintException(Exception e)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message + "\n\n\tBack");
            Console.ReadKey();
            Console.ResetColor();
        }
    }
}
