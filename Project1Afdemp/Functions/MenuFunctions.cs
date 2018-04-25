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
                    List<ChatMessage> chatMessages = database.Chat.Include("UnreadUsers").OrderBy(i => i.Id).ToList();

                    bool firstNewMessage = true;

                    foreach (ChatMessage message in chatMessages)
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

        public static void PersonalMessages(UserManager activeUserManager)
        {
            while(true)
            {
                using (var database = new DatabaseStuff())
                {
                    // Probe the database for the sent and received personal messages
                    List<Message> receivedMessages  = database.Messages.Where(message => message.Receiver.Id == activeUserManager.TheUser.Id).ToList();
                    List<Message> sentMessages      = database.Messages.Where(message => message.Sender.Id == activeUserManager.TheUser.Id).ToList();

                    int countReceivedMessages       = receivedMessages.Count;
                    int countUnreadReceivedMessages = receivedMessages.Count(message => message.IsRead == false);
                    int countSentMessages           = sentMessages.Count;
                    int countUnreadSentMessages     = sentMessages.Count(message => message.IsRead == false);

                    string inbox = $"Inbox ({countUnreadReceivedMessages}/{countReceivedMessages})";
                    string sent  = $"Sent  ({countUnreadSentMessages}/{countSentMessages})";
                    

                    // Create the Message Menu items common to all users
                    List<string> messageMenuItems = new List<string> { $"Create NEW", $"{inbox}", $"{sent}", "Back" };

                    // Acquire the choice of function from the user using a vertical menu
                    string userChoice = Menus.VerticalMenu(StringsFormatted.PersonalMessages, messageMenuItems);

                    if (userChoice.Contains("Create"))
                    {
                        PersonalMessageFunctions.SendEmail(activeUserManager);
                    }
                    else if (userChoice.Contains("Inbox"))
                    {
                        PersonalMessageFunctions.PresentAndManipulateMessage(activeUserManager, receivedMessages);
                    }
                    else if (userChoice.Contains("Sent"))
                    {
                        PersonalMessageFunctions.PresentAndManipulateMessage(activeUserManager, sentMessages, Received: false);
                    }
                    else
                    {
                        return;
                    }
                }
            }
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
                        Console.WriteLine($"\t{message.TimeSent.ToString("dd/MM HH:mm")} {senderName} sent '{message.Title}' to {receiverName}");
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
                ManageUserFunctions.CreateNewUser();
                return;
            }
            else if (manageUsersChoice.Contains("Back"))
            {
                return;
            }
            User receiver = ManageUserFunctions.SelectUser(activeUserManager);
            if (receiver is null) { return; }
            if (manageUsersChoice == "Delete User")
            {
                ManageUserFunctions.DeleteUser(receiver);
            }
            else if (manageUsersChoice == "Permissions")
            {
                ManageUserFunctions.ChangeUserPermissions(receiver);
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
