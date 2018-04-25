using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    class PersonalMessageFunctions
    {
        public static void SendEmail(UserManager activeUserManager, User receiver = null, string replyTitle = "")
        {
            string title = "\n\n\tTitle: " + replyTitle;

            // NULL Receiver means it's NOT a REPLY it's just a NEW message.
            if (receiver is null) { receiver = ManageUserFunctions.SelectUser(activeUserManager); }
            if (receiver is null) { return; }

            Console.WriteLine(StringsFormatted.SendEmail);
            Console.Write(title);

            string MessageTitle = (title.Contains("RE")) ? replyTitle : Console.ReadLine();

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
                catch (Exception e) { MenuFunctions.PrintException(e); }
                Console.ReadKey();
            }
        }

        public static void PresentAndManipulateMessage(UserManager activeUserManager, List<Message> Messages, bool Received = true)
        {
            string userChoice;
            do
            {
                Message selectedMessage = SelectMessage(activeUserManager, Messages, Received);

                if (selectedMessage is null) { return; }

                string presentedMessage = StringsFormatted.ReadEmails
                                        + $"\n\n\tTitle: {selectedMessage.Title}"
                                        + $"\n\n\tBody:  {selectedMessage.Body}\n\n";

                List<string> messageOptions = new List<string> { "Forward", "Delete", "Back" };

                if (Received)
                { messageOptions.Insert(0, "Reply"); }
                else
                { messageOptions.Insert(0, "Edit"); }

                userChoice = Menus.HorizontalMenu(presentedMessage, messageOptions);

                using (var database = new DatabaseStuff())
                {
                    Message readMessage = database.Messages.Single(m => m.Id == selectedMessage.Id);
                    if (Received) { readMessage.IsRead = true; }
                    database.SaveChanges();
                    if (userChoice.Contains("Forward"))
                    {
                        ForwardMessage(activeUserManager, selectedMessage);
                    }
                    else if (userChoice.Contains("Reply"))
                    {
                        User toBeReplied = database.Users.Single(u => u.Id == readMessage.Sender.Id);
                        SendEmail(activeUserManager, toBeReplied, "RE> " + readMessage.Title);
                    }
                    else if (userChoice.Contains("Edit"))
                    {
                        UpdateEmail(activeUserManager, selectedMessage);
                    }
                    else if (userChoice.Contains("Delete"))
                    {
                        DeleteMessage(selectedMessage);
                        return;
                    }
                }
            }
            while (!userChoice.Contains("Back"));
        }

        public static Message SelectMessage(UserManager activeUserManager, List<Message> Messages, bool Received)
        {
            List<string> selectMessageItems = new List<string>();
            string receiverName;
            using (var database = new DatabaseStuff())
            {
                //List<Message> messages = database.Messages.ToList();
                int UserId = database.Users.Single(i => i.UserName == activeUserManager.UserName).Id;

                try
                {
                    foreach (Message message in Messages)
                    {
                        if (Received)
                        {
                            if (message.ReceiverId == UserId)
                            {
                                receiverName = database.Users.Single(i => i.Id == message.SenderId).UserName;
                                selectMessageItems.Add(CustomizeAppearanceOfMessage(message, receiverName, Received));
                            }
                        }
                        else
                        {
                            if (message.SenderId == UserId)
                            {
                                receiverName = database.Users.Single(i => i.Id == message.ReceiverId).UserName;
                                selectMessageItems.Add(CustomizeAppearanceOfMessage(message, receiverName, Received));
                            }
                        }
                    }
                }
                catch (Exception e) { MenuFunctions.PrintException(e); }

                if (selectMessageItems.Count == 0)
                {
                    Console.WriteLine("\n\n\tNo Messages to View");
                    Console.ReadKey();
                    return null;
                }
                selectMessageItems.Add("Back");
                string oMessage = Menus.VerticalMenu(StringsFormatted.OpenMessage, selectMessageItems);
                if (oMessage.Contains("Back"))
                {
                    return null;
                }
                int messageID = int.Parse(oMessage.Split('|')[1]);

                Console.Clear();
                return database.Messages.Single(i => i.Id == messageID);
            }
        }

        public static string CustomizeAppearanceOfMessage(Message message, string receiverName, bool Received)
        {
            string direction     = (Received)       ? "From:" : "To:";
            string listedMessage = (message.IsRead) ? ""      : "* ";

            listedMessage += $"{direction} {receiverName} Title: '{message.Title}' Time Sent: {message.TimeSent.ToString("dd/MM HH:mm")} ID: |{message.Id}|";
            return listedMessage;
        }

        public static void UpdateEmail(UserManager activeUserManager, Message editMessage)
        {
            Console.Write("\n\n\n\n\tNew Title: ");
            string newMessageTitle = Console.ReadLine();

            Console.Write("\n\tNew Body: ");
            string newMessageBody = Console.ReadLine();

            using (var database = new DatabaseStuff())
            {
                try
                {
                    Message newMessage = database.Messages.Single(m => m.Id == editMessage.Id);
                    newMessage.Title = newMessageTitle;
                    newMessage.Body = newMessageBody;
                    newMessage.IsRead = false;
                    database.SaveChanges();
                    Console.Write($"\n\n\tEmail updated successfully\n\n\tOK");
                }
                catch (Exception e) { MenuFunctions.PrintException(e); }
                Console.ReadKey(true);
            }
        }

        public static void ForwardMessage(UserManager activeUserManager, Message forwardMessage)
        {
            User receiver = ManageUserFunctions.SelectUser(activeUserManager);
            if (receiver is null) { return; }
            string forwardTitle = "FW:" + forwardMessage.Title;
            string forwardBody = forwardMessage.Body;
            Message forwardedMessage = new Message(activeUserManager.TheUser.Id, receiver.Id, forwardTitle, forwardBody);
            using (var database = new DatabaseStuff())
            {
                database.Messages.Add(forwardedMessage);
                database.SaveChanges();
            }
            Console.Clear();
            Console.WriteLine($"\n\n\tMessage successfully forwarded to {receiver.UserName}\n\n\tOK");
            Console.ReadKey();
        }

        public static void DeleteMessage(Message receivedMessage)
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete this message?", new List<string> { "Yes", "No" }).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    database.Messages.Remove(database.Messages.Single(i => i.Id == receivedMessage.Id));
                    database.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("\n\n\tMessage successfully DELETED\n\n\tOK");
                    Console.ReadKey();
                }
            }
        }
    }
}
